using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using FluentResults;
using FluentResults.Extensions;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.UserActions;
using Momentum.Analytics.Core.UserActions.Interfaces;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb.UserActions;

public class FieldChangeStorage : ResourceStorageBase<IFieldChangeTableConfiguration>, IFieldChangeStorage
{
    public FieldChangeStorage(
        IDynamoDBClientFactory clientFactory,
        IFieldChangeTableConfiguration configuration,
        ILogger<FieldChangeStorage> logger) : base(clientFactory, configuration, logger)
    {
    } // end method
    public Task<Result> InsertAsync(FieldChange fieldChange, CancellationToken cancellationToken = default)
    {
        var request = new PutItemRequest
        {
            TableName = _tableConfiguration.TableName,
            Item = fieldChange.ToDynamoDb(),
        };

        return Result.Try(() => _clientFactory.GetAsync(cancellationToken))
            .Bind(client => 
            {
                if(client == null)
                {
                    return Task.FromResult(Result.Fail<PutItemResponse>("DynamoDB client is null"));
                }
                return Result.Try(() => client.PutItemAsync(request, cancellationToken));
            })
            .Bind(response => {
                if(response.HttpStatusCode == HttpStatusCode.OK)
                {
                    return Result.Ok();
                }

                return Result.Fail("Failed to insert field change");
            });
    } // end method

    public virtual Task<Result<FieldChange>> GetLatestAsync(Guid cookieId, CancellationToken cancellationToken = default)
    {
        var request = new QueryRequest()
        {
            TableName = _tableConfiguration.TableName,
            KeyConditions = new Dictionary<string, Condition>()
            {
                { 
                    FieldChangeConstants.COOKIE_ID, 
                    new Condition()
                    {
                        AttributeValueList = new List<AttributeValue>()
                        {
                            new AttributeValue(cookieId.ToString())
                        },
                        ComparisonOperator = ComparisonOperator.EQ
                    }
                }
            },
            ScanIndexForward = false,
            Limit = 1
        };

        return Result.Try(() => _clientFactory.GetAsync(cancellationToken))
            .Bind(client => {
                if(client == null)
                {
                    return Task.FromResult(Result.Fail<QueryResponse>("DynamoDB client is null"));
                }
                return Result.Try(() => client.QueryAsync(request, cancellationToken));
            })
            .Bind(response => {
                if(response.HttpStatusCode == HttpStatusCode.OK)
                {
                    if(response.Items != null && response.Items.Any())
                    {
                        return Result.Ok(response.Items.First().ToFieldChange());
                    }
                    return Result.Fail<FieldChange>($"No field change found for cookie id: {cookieId}");
                }
                return Result.Fail("Failed to query field changes");
            });
    } // end method
} // end class

public static class FieldChangeExtensions
{
    public static Dictionary<string, AttributeValue> ToDynamoDb(this FieldChange fieldChange)
    {
        return new Dictionary<string, AttributeValue>()
            .AddField(FieldChangeConstants.COOKIE_ID, fieldChange.CookieId)
            .AddField(FieldChangeConstants.VISIT_ID, fieldChange.VisitId.ToString())
            .AddField(FieldChangeConstants.UTC_TIMESTAMP, fieldChange.UtcTimestamp)
            .AddField(FieldChangeConstants.URL, fieldChange.Url)
            .AddField(FieldChangeConstants.FIELD, fieldChange.Field)
            .AddField(FieldChangeConstants.VALUE, fieldChange.Value);
    } // end method

    public static FieldChange ToFieldChange(this Dictionary<string, AttributeValue> fields)
    {
        return new FieldChange
        {
            CookieId = fields.ReadGuid(FieldChangeConstants.COOKIE_ID, true)!.Value,
            VisitId = fields.ReadUlid(FieldChangeConstants.VISIT_ID),
            UtcTimestamp = fields.ReadDateTime(FieldChangeConstants.UTC_TIMESTAMP, true)!.Value,
            Url = fields.ReadRequiredString(FieldChangeConstants.URL),
            Field = fields.ReadRequiredString(FieldChangeConstants.FIELD),
            Value = fields.ReadString(FieldChangeConstants.VALUE) ?? string.Empty
        };
    } // end method
} // end class