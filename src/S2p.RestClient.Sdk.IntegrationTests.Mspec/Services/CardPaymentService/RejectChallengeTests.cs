﻿using System;
using System.Net;
using System.Threading.Tasks;
using Machine.Specifications;
using S2p.RestClient.Sdk.Entities;
using S2p.RestClient.Sdk.Infrastructure;

namespace S2p.RestClient.Sdk.IntegrationTests.Mspec.Services.CardPaymentService
{
   partial class CardPaymentServiceTests
    {
        [Subject(typeof(Sdk.Services.CardPaymentService))]
        public class When_rejecting_challenge
        {
            private static ApiResult<ApiCardPaymentResponse> RejectChallengeResult;

            private Establish context = () => {
                ServiceTestsConstants.EnableTLS12();
                InitializeHttpBuilder();
                HttpClient = HttpClientBuilder.Build();
                CardPaymentService = new Sdk.Services.CardPaymentService(HttpClient, BaseAddress);
                CardPaymentRequest = new ApiCardPaymentRequest
                {
                    Payment = new CardPaymentRequest
                    {
                        MerchantTransactionID = MerchantTransactionID,
                        Amount = 100,
                        Currency = "USD",
                        ReturnURL = "http://demo.smart2pay.com/redirect.php",
                        Description = DescriptionText,
                        StatementDescriptor = "card payment",
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
                        ShippingAddress = null,
                        Customer = new Customer
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "challenge@challenge.com",
                            SocialSecurityNumber = "00003456789"
                        },
                        Card = new CardDetailsRequest
                        {
                            HolderName = "John Doe",
                            Number = "4111111111111111",
                            ExpirationMonth = "02",
                            ExpirationYear = "2022",
                            SecurityCode = "312"
                        },
                        Capture = false,
                        Retry = false,
                        GenerateCreditCardToken = false,
                        PaymentTokenLifetime = 100,
                        ThreeDSecureCheck = false,
                        Language = "ro-RO",
                        SkinID = 200
                    }
                };
            };

            private Because of = () => {
                RejectChallengeResult = BecauseAsync().GetAwaiter().GetResult();
            };

            private static async Task<ApiResult<ApiCardPaymentResponse>> BecauseAsync()
            {
                ApiResult = await CardPaymentService.InitiatePaymentAsync(CardPaymentRequest);
                return await CardPaymentService.RejectChallengeAsync(ApiResult.Value.Payment.ID.Value);
            }

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

            private It should_have_the_correct_status_id = () => {
                ApiResult.Value.Payment.Status.ID.ShouldEqual(CardPaymentStatusDefinition.PendingChallengeConfirmation);
            };

            private It should_have_the_correct_status_info = () => {
                ApiResult.Value.Payment.Status.Info.ShouldEqual(nameof(CardPaymentStatusDefinition.PendingChallengeConfirmation));
            };

            private It should_have_the_correct_installments_number = () => {
                ApiResult.Value.Payment.Installments.ShouldEqual(CardPaymentRequest.Payment.Installments);
            };

            private It should_have_the_correct_fraud_status = () => {
                ApiResult.Value.Payment.Fraud.Status.ShouldEqual(CardFraudStatusDefinition.Challenge);
            };

            private It should_have_the_correct_check_mode = () => {
                ApiResult.Value.Payment.Fraud.CheckMode.ShouldEqual(CardFraudCheckModeDefinition.CheckOnPreAuthorisation);
            };

            private It should_have_score_greater_than_zero = () => {
                ApiResult.Value.Payment.Fraud.Score.ShouldBeGreaterThan(0);
            };

            private It should_have_not_null_fraud_reason = () => {
                String.IsNullOrWhiteSpace(ApiResult.Value.Payment.Fraud.Reason).ShouldBeFalse();
            };

            private It should_have_correct_status_id_after_reject = () => {
                RejectChallengeResult.Value.Payment.Status.ID.ShouldEqual(CardPaymentStatusDefinition.Cancelled);
            };

            private It should_have_the_correct_status_info_after_reject = () => {
                RejectChallengeResult.Value.Payment.Status.Info.ShouldEqual(nameof(CardPaymentStatusDefinition.Cancelled));
            };

            private It should_have_ok_bad_request_for_reject = () => {
                RejectChallengeResult.HttpResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);
            };
        }
    }
}