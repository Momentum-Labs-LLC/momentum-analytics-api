resource "aws_api_gateway_rest_api" "this-api-0" {
  name        = local.name_prefix
  description = "A microservice for ${local.api_name} in the ${local.env} environment."
  body = jsonencode(
    {
      "openapi" : "3.0.1"
      "info" : {
        "title" : "${local.api_name}"
        "description" : "A microservice for ${local.api_name} in the ${local.env} environment."
        "version" : "v1"
      },
      "paths" : {
        "/{proxy+}" : {
          "x-amazon-apigateway-any-method" : {
            "produces" : [
              "*/*"
            ],
            "parameters" : [
              {
                "name" : "proxy"
                "in" : "path"
                "required" : true
                "type" : "string"
              }
            ],
            "responses" : {}
            "x-amazon-apigateway-integration" : {
              "uri" : "arn:aws:apigateway:${local.region}:lambda:path/2015-03-31/functions/arn:aws:lambda:${local.region}:${local.aws_account_id}:function:${local.api_name}/invocations"
              "responses" : {
                "default" : {
                  "statusCode" : "200"
                }
              },
              "passthroughBehavior" : "when_no_match"
              "httpMethod" : "POST"
              "contentHandling" : "CONVERT_TO_TEXT"
              "type" : "aws_proxy"
            }
          }
        }
      }
    }
  ) # end jsonencode

  endpoint_configuration {
    types = [local.endpoint_type]
  }

  tags = merge(local.tags,
    tomap(
      {
        "Name" = local.api_name
      }
  ))

  depends_on = [
    aws_lambda_function.this-api-0
  ]
}

resource "aws_api_gateway_deployment" "this-api-deploy" {
  rest_api_id = aws_api_gateway_rest_api.this-api-0.id
  stage_name = local.env

  # redeploy whenever the s3 etag is differnt
  triggers = {
    redeployment = sha1(jsonencode([
      aws_api_gateway_rest_api.this-api-0.body,
      data.aws_ecr_image.this-image.image_tag
    ]))
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_api_gateway_stage" "this-api-stage" {
  deployment_id = aws_api_gateway_deployment.this-api-deploy.id
  rest_api_id   = aws_api_gateway_rest_api.this-api-0.id
  stage_name    = local.env
}

resource "aws_lambda_permission" "this-lambda-permission" {
  statement_id  = "AllowExecutionFromAPIGateway"
  action        = "lambda:InvokeFunction"
  principal     = "apigateway.amazonaws.com"
  function_name = aws_lambda_function.this-api-0.function_name

  source_arn = "arn:aws:execute-api:${local.region}:${local.aws_account_id}:${aws_api_gateway_rest_api.this-api-0.id}/*"

  depends_on = [
    aws_lambda_function.this-api-0,
    aws_api_gateway_rest_api.this-api-0,
    #aws_api_gateway_deployment.this-api-deploy
  ]
}

#setup the custom domain name for the api gateway
# resource "aws_api_gateway_domain_name" "this-domain-0" {
#   domain_name              = local.custom_domain_name
#   regional_certificate_arn = data.aws_acm_certificate.this-cert.arn
#   security_policy          = "TLS_1_2"

#   endpoint_configuration {
#     types = [local.endpoint_type]
#   }

#   tags = local.tags
# }

# resource "aws_api_gateway_base_path_mapping" "this-mapping-0" {
#   api_id      = aws_api_gateway_rest_api.this-api-0.id
#   stage_name  = aws_api_gateway_deployment.this-api-deploy.stage_name
#   domain_name = aws_api_gateway_domain_name.this-domain-0.domain_name

#   depends_on = [
#     aws_api_gateway_rest_api.this-api-0,
#     aws_api_gateway_deployment.this-api-deploy,
#     aws_api_gateway_domain_name.this-domain-0,
#     aws_route53_record.this-route
#   ]
# }