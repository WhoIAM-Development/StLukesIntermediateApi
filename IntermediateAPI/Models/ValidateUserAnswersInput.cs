﻿using IntermediateAPI.Models.External.Experian;

namespace IntermediateAPI.Models
{
    public class ValidateUserAnswersInput
    {
        public string? SessionId { get; set; }
        public string? AnswerIndex { get; set; }
        public int ProfileId { get; set; }
    }
}