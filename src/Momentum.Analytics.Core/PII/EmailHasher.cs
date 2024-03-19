using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII.Interfaces;

namespace Momentum.Analytics.Core.PII
{
    public class EmailHasher : IEmailHasher
    {
        protected readonly ILogger _logger;

        public EmailHasher(ILogger<EmailHasher> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public virtual async Task<string> HashEmailAsync(string email, CancellationToken token = default)
        {
            // Convert byte array to a string
            var result = new StringBuilder();

            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(email));
                
                for (int i = 0; i < bytes.Length; i++)
                {
                    result.Append(bytes[i].ToString("x2"));
                } // end for                
            } // end using

            return result.ToString();
        } // end method
    } // end class
} // end namespace