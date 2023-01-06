using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Social.Services.Helpers
{
    public class CustomBadRequest : ValidationProblemDetails
    {
        public string Message { get; set; }
        public int statusCode { get; set; }
        public bool isSuccessful { get; set; }
        public CustomBadRequest(ActionContext context)
        {
           // Title = "Invalid arguments to the API";
           // Detail = "The inputs supplied to the API are invalid";
            Status = 406;
            ConstructErrorMessages(context);
            Message = Errors.Values.FirstOrDefault()?.FirstOrDefault();
            statusCode = 406;
            isSuccessful = false;
            Type = context.HttpContext.TraceIdentifier;
        }

        private void ConstructErrorMessages(ActionContext context)
        {
            foreach (var keyModelStatePair in context.ModelState)
            {
                var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;
                if (errors != null && errors.Count > 0)
                {
                    if (errors.Count == 1)
                    {
                        var errorMessage = GetErrorMessage(errors[0]);
                        Errors.Add(key, new[] { errorMessage });
                    }
                    else
                    {
                        var errorMessages = new string[errors.Count];
                        for (var i = 0; i < errors.Count; i++)
                        {
                            errorMessages[i] = GetErrorMessage(errors[i]);
                        }

                        Errors.Add(key, errorMessages);
                    }
                }
            }
        }

        string GetErrorMessage(ModelError error)
        {
            return string.IsNullOrEmpty(error.ErrorMessage) ?
                "The input was not valid." :
            error.ErrorMessage;
        }
    }
}
