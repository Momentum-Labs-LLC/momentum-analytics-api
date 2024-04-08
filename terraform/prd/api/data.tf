data "aws_vpc" "this-vpc" {
  tags = {
    Name = local.vpc_name
  }
}

data "aws_subnets" "private" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.this-vpc.id]
  }
}

# data "aws_acm_certificate" "this-cert" {
#   domain      = local.full_domain
#   types       = ["AMAZON_ISSUED"]
#   statuses    = ["ISSUED"]
#   most_recent = true
# }

# data "aws_route53_zone" "this-zone" {
#   name = local.zone_name
# }

data "aws_ecr_image" "this-image" {
  repository_name = "${local.corp}-${local.project}-${local.subproject}-repo"
  image_tag       = var.api_image_tag
}

data "aws_dynamodb_table" "this-page-views" {
  name = "${local.corp}-${local.env}-page-views"
}

data "aws_dynamodb_table" "this-visits" {
  name = "${local.corp}-${local.env}-visits"
}

data "aws_dynamodb_table" "this-pii" {
  name = "${local.corp}-${local.env}-pii-values"
}

data "aws_dynamodb_table" "this-collected-pii" {
  name = "${local.corp}-${local.env}-collected-pii"
}

data "aws_cloudfront_distribution" "this-distribution" {
  id = "E3NXINRKUEXAOJ"
}

data "aws_acm_certificate" "cert" {
  domain   = "mll-analytics.com"
  statuses = ["ISSUED"]
}

data "aws_route53_zone" "this-zone" {
  name = "mll-analytics.com"
}