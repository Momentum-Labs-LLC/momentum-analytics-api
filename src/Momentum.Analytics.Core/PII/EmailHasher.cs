using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Momentum.Analytics.Core.PII.Interfaces;

namespace Momentum.Analytics.Core.PII
{
    public class EmailHasher : IEmailHasher
    {
        public const string EMAIL_TO_UPPER = "EMAIL_TO_UPPERCASE";
        public const bool EMAIL_TO_UPPER_DEFAULT = false;
        protected readonly bool _toUpperCase;
        protected readonly ILogger _logger;

        public EmailHasher(IConfiguration configuration, ILogger<EmailHasher> logger)
        {
            _toUpperCase = configuration.GetValue<bool>(EMAIL_TO_UPPER, EMAIL_TO_UPPER_DEFAULT);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task<string> HashEmailAsync(string email, CancellationToken token = default)
        {
            // Convert byte array to a string
            var result = new StringBuilder();

            var emailToHash = email.Trim();
            if(_toUpperCase)
            {
                emailToHash = emailToHash.ToUpperInvariant();
            }
            else
            {
                emailToHash = emailToHash.ToLowerInvariant();
            } // end if

            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(emailToHash));
                
                for (int i = 0; i < bytes.Length; i++)
                {
                    result.Append(bytes[i].ToString("x2"));
                } // end for                
            } // end using

            return result.ToString();
        } // end method
    } // end class
} // end namespace