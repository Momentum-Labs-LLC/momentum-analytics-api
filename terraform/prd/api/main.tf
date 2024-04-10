provider "aws" {
  region = local.region
}

data "aws_availability_zones" "available" {}

data "aws_caller_identity" "current" {}

locals {
  env            = "prd"
  corp           = "momentum"
  iteration      = 0
  region         = "us-east-1"
  project        = "analytics"
  subproject     = "api"
  aws_account_id = data.aws_caller_identity.current.account_id
  vpc_name       = "${local.corp}-${local.env}-vpc-0"
  name_prefix    = "${local.corp}-${local.env}-${local.project}-${local.subproject}"
  api_name       = "${local.name_prefix}-${local.iteration}"

  endpoint_type   = "REGIONAL"
  timeout         = 60
  memory_size     = 1024
  max_concurrency = -1

  log_retention_days = 30

  #alerts
  api_error_rate_period    = 5 * 60
  api_error_rate_periods   = 3
  api_error_rate_threshold = 2

  api_request_period    = 1 * 60
  api_request_periods   = 3
  api_request_threshold = 1


  # ingress/egress/security group stuff
  this_ipv4_cidr_blocks = [data.aws_vpc.this-vpc.cidr_block]
  this_ipv6_cidr_blocks = [data.aws_vpc.this-vpc.ipv6_cidr_block]

  tags = {
    Production  = "true"
    Environment = local.env
    Project     = "${local.project}-${local.subproject}"
  }
}



