﻿using System;
using System.Net;
using System.Net.Http;
using Machine.Specifications;
using S2p.RestClient.Sdk.Entities;
using S2p.RestClient.Sdk.Infrastructure;
using S2p.RestClient.Sdk.Services;

namespace S2p.RestClient.Sdk.IntegrationTests.Mspec.Services.CardPaymentService
{
    public partial class CardPaymentServiceTests
    {
        private static ICardPaymentService CardPaymentService;

        private static ApiResult<ApiCardPaymentResponse> ApiResult;
        private static string MerchantTransactionID => Guid.NewGuid().ToString();
        private static ApiCardPaymentRequest CardPaymentRequest;
        private static IHttpClientBuilder HttpClientBuilder;
        private static HttpClient HttpClient;
        private static Uri BaseAddress = new Uri(ServiceTestsConstants.CardPaymentSystemBaseUrl);
        private const string DescriptionText = "SDK Test Payment";
        private const int CreditCardMethodID = 6;

        private static void InitializeHttpBuilder()
        {
            HttpClientBuilder = new HttpClientBuilder(() => ServiceTestsConstants.CardPaymentSystemAuthenticationConfiguration);
        }


        [Subject(typeof(Sdk.Services.CardPaymentService))]
        public class When_creating_a_card_payment_with_card_details
        {
            private Establish context = () => {
                InitializeHttpBuilder();
                HttpClient = HttpClientBuilder.Build();
                CardPaymentService = new Sdk.Services.CardPaymentService(HttpClient, BaseAddress);
                CardPaymentRequest = new CardPaymentRequest
                {
                    MerchantTransactionID = MerchantTransactionID,
                    Amount = 9000,
                    Currency = "USD",
                    ReturnURL = "http://demo.smart2pay.com/redirect.php",
                    Description = DescriptionText,
                    StatementDescriptor = "bank statement message",
                    Card = new CardDetailsRequest
                    {
                        HolderName = "John Doe",
                        Number = "4111111111111111",
                        ExpirationMonth = "02",
                        ExpirationYear = "2022",
                        SecurityCode = "312"
                    },
                    BillingAddress = new Address
                    {
                        City = "Iasi",
                        ZipCode = "7000-49",
                        State = "Iasi",
                        Street = "Sf Lazar",
                        StreetNumber = "37",
                        HouseNumber = "5A",
                        HouseExtension = "-",
                        Country = "BR"
                    },
                    Capture = true,
                    Retry = false,
                    GenerateCreditCardToken = false,
                    PaymentTokenLifetime = 5
                }.ToApiCardPaymentRequest();
            };

            private Because of = () => {
                ApiResult = CardPaymentService.CreatePaymentAsync(CardPaymentRequest).GetAwaiter().GetResult();
            };

            private Cleanup after = () => { HttpClient.Dispose(); };

            private It should_have_created_status_code = () => {
                ApiResult.HttpResponse.StatusCode.ShouldEqual(HttpStatusCode.Created);
            };

            private It should_have_the_same_merchant_transaction_id = () => {
                ApiResult.Value.Payment.MerchantTransactionID.ShouldEqual(CardPaymentRequest.Payment.MerchantTransactionID);
            };

            private It should_have_the_correct_amount = () => {
                ApiResult.Value.Payment.Amount.ShouldEqual(CardPaymentRequest.Payment.Amount);
            };

            private It should_have_the_same_currency = () => {
                ApiResult.Value.Payment.Currency.ShouldEqual(CardPaymentRequest.Payment.Currency);
            };

            private It should_have_the_correct_method_id = () => {
                ApiResult.Value.Payment.MethodID.ShouldEqual(CreditCardMethodID);
            };

            private It should_have_null_redirect_url = () => {
                String.IsNullOrWhiteSpace(ApiResult.Value.Payment.RedirectURL).ShouldBeTrue();
            };


            private It should_have_the_correct_status_id = () => {
                ApiResult.Value.Payment.Status.ID.ShouldEqual(CardPaymentStatusDefinition.CaptureRequested);
            };

            private It should_have_the_correct_status_info = () => {
                ApiResult.Value.Payment.Status.Info.ShouldEqual(nameof(CardPaymentStatusDefinition.CaptureRequested));
            };

            private It should_have_the_correct_card_holder_name = () => {
                ApiResult.Value.Payment.Card.HolderName.ShouldEqual(CardPaymentRequest.Payment.Card.HolderName);
            };

            private It should_have_the_correct_card_number = () => {
                string requestCardNumber = CardPaymentRequest.Payment.Card.Number;
                string responseCardNumber = ApiResult.Value.Payment.Card.Number;
                responseCardNumber.Substring(responseCardNumber.Length-4).ShouldEqual(requestCardNumber.Substring(requestCardNumber.Length-4));
            };

            private It should_have_the_correct_expiration_month = () => {
                ApiResult.Value.Payment.Card.ExpirationMonth.ShouldEqual(CardPaymentRequest.Payment.Card.ExpirationMonth);
            };

            private It should_have_the_correct_expiration_year = () => {
                ApiResult.Value.Payment.Card.ExpirationYear.ShouldEqual(CardPaymentRequest.Payment.Card.ExpirationYear);
            };
        }
    }
}
