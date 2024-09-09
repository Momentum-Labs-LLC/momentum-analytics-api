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
  subproject     = "pii-processing"
  aws_account_id = data.aws_caller_identity.current.account_id
  vpc_name       = "${local.corp}-${local.env}-vpc-0"
  name_prefix    = "${local.corp}-${local.env}-${local.project}-${local.subproject}"
  function_name  = "${local.name_prefix}-${local.iteration}"
  dlq_name       = "${local.name_prefix}-dlq-${local.iteration}"

  timeout         = 60
  memory_size     = 1024
  max_concurrency = -1

  log_retention_days = 30

  # alerts
  behind_period    = 5 * 60         # 5 minutes
  behind_periods   = 3              # 15 minutes
  behind_threshold = 1000 * 60 * 30 # 30 minutes

  # lambda dynamodb event source mapping
  db_batch_size           = 10
  db_batching_window      = 30
  db_split_batch_on_error = true
  db_max_retries          = 1

  # lambda DLQ event source mapping
  dlq_enabled            = false
  dlq_batch_size         = 1
  dlq_delay              = 60  // 1 minute
  dlq_visibility_timeout = 300 // 5 mintues

  tags = {
    Production  = "true"
    Environment = local.env
    Project     = "${local.project}-${local.subproject}"
  }
}



