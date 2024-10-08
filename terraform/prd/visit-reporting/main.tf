provider "aws" {
  region = local.region

  default_tags {
    tags = {
      Production  = "true"
      Environment = local.env
      Project     = "${local.project}-${local.subproject}"
    }
  }
}

data "aws_availability_zones" "available" {}

data "aws_caller_identity" "current" {}

locals {
  env            = "prd"
  corp           = "momentum"
  iteration      = 0
  region         = "us-east-1"
  project        = "analytics"
  subproject     = "visit-reporting"
  aws_account_id = data.aws_caller_identity.current.account_id
  vpc_name       = "${local.corp}-${local.env}-vpc-0"
  name_prefix    = "${local.corp}-${local.env}-${local.project}-${local.subproject}"
  function_name  = "${local.name_prefix}-${local.iteration}"

  timeout         = 900
  memory_size     = 3008
  max_concurrency = -1

  log_retention_days = 365

  tags = {
    Production  = "true"
    Environment = local.env
    Project     = "${local.project}-${local.subproject}"
  }
}



