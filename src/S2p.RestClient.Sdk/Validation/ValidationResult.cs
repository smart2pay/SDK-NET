﻿namespace S2p.RestClient.Sdk.Validation
{
    public class ValidationResult
    {
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public int ErrorsCount { get; set; } = 0;
    }
}
