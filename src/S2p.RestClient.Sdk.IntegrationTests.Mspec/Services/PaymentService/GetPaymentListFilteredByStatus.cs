﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;
using S2p.RestClient.Sdk.Entities;
using S2p.RestClient.Sdk.Infrastructure;

namespace S2p.RestClient.Sdk.IntegrationTests.Mspec.Services.PaymentService
{
    public partial class PaymentServiceTests
    {
        [Subject(typeof(Sdk.Services.PaymentService))]
        public class When_requesting_payment_list_filtered_by_status
        {
            private static ApiResult<ApiPaymentListResponse> ApiListResult;
            private static PaymentsFilter PaymentsFilter;
            protected static PaymentFilteredBehaviourData Data;
            private const int Limit = 6;
            private static readonly DateTime StartDate = new DateTime(2018, 4, 3);
            private static readonly DateTime EndDate = new DateTime(2018, 4, 4);
            private static readonly int StatusId = 9;


            private Establish context = () => {
                InitializeHttpBuilder();
                HttpClient = HttpClientBuilder.Build();
                PaymentService = new Sdk.Services.PaymentService(HttpClient, BaseAddress);
                PaymentsFilter = new PaymentsFilter { startDate = StartDate, endDate = EndDate, pageSize = 100, statusID = StatusId };
            };

            private Because of = () => {
                ApiListResult = PaymentServiceTests.PaymentService.GetPaymentListAsync(PaymentsFilter).GetAwaiter()
                    .GetResult();
                Data = new PaymentFilteredBehaviourData
                {
                    ApiListResult = ApiListResult,
                    Limit = Limit,
                    PaymentsFilter = PaymentsFilter
                };
            };

            private Cleanup after = () => { HttpClient.Dispose(); };

            Behaves_like<PaymentFilteredBehavior> a_list_of_filtered_payments_response;
        }
    }
}