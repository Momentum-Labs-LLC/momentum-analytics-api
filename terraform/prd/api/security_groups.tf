# resource "aws_security_group" "this-local-0" {
#   name   = "${local.name_prefix}-local-0"
#   vpc_id = data.aws_vpc.this-vpc.id

#   ingress {
#     cidr_blocks      = local.this_ipv4_cidr_blocks
#     description      = "All - ICMPv4"
#     from_port        = -1
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "icmp"
#     security_groups  = []
#     self             = true
#     to_port          = -1
#   }
#   ingress {
#     cidr_blocks      = []
#     description      = "All - ICMPv6"
#     from_port        = -1
#     ipv6_cidr_blocks = local.this_ipv6_cidr_blocks
#     prefix_list_ids  = []
#     protocol         = "icmpv6"
#     security_groups  = []
#     self             = true
#     to_port          = -1
#   }
#   ingress {
#     cidr_blocks      = local.this_ipv4_cidr_blocks
#     description      = "Local - TCP"
#     from_port        = 0
#     ipv6_cidr_blocks = local.this_ipv6_cidr_blocks
#     prefix_list_ids  = []
#     protocol         = "tcp"
#     security_groups  = []
#     self             = true
#     to_port          = 0
#   }
#   ingress {
#     cidr_blocks      = local.this_ipv4_cidr_blocks
#     description      = "Local - UDP"
#     from_port        = 0
#     ipv6_cidr_blocks = local.this_ipv6_cidr_blocks
#     prefix_list_ids  = []
#     protocol         = "udp"
#     security_groups  = []
#     self             = true
#     to_port          = 0
#   }
#   ingress {
#     cidr_blocks      = local.this_ipv4_cidr_blocks
#     description      = "Local - ALL"
#     from_port        = 0
#     ipv6_cidr_blocks = local.this_ipv6_cidr_blocks
#     prefix_list_ids  = []
#     protocol         = -1
#     security_groups  = []
#     self             = true
#     to_port          = 0
#   }



#   egress {
#     cidr_blocks = [
#       "0.0.0.0/0"
#     ]
#     description      = "All - ICMPv4"
#     from_port        = -1
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "icmp"
#     security_groups  = []
#     self             = true
#     to_port          = -1
#   }
#   egress {
#     cidr_blocks = []
#     description = "All - ICMPv6"
#     from_port   = -1
#     ipv6_cidr_blocks = [
#       "::/0"
#     ]
#     prefix_list_ids = []
#     protocol        = "icmpv6"
#     security_groups = []
#     self            = true
#     to_port         = -1
#   }
#   egress {
#     cidr_blocks = [
#       "0.0.0.0/0"
#     ]
#     description = "All - All"
#     from_port   = 0
#     ipv6_cidr_blocks = [
#       "::/0"
#     ]
#     prefix_list_ids = []
#     protocol        = "-1"
#     security_groups = []
#     self            = true
#     to_port         = 0
#   }

#   tags = merge(local.tags,
#     tomap(
#       {
#         "Name" = "${local.name_prefix}-local-0"
#       }
#   ))
# }

# resource "aws_security_group" "this-self-0" {
#   name   = "${local.name_prefix}-self-0"
#   vpc_id = data.aws_vpc.this-vpc.id
#   #ingress = local.sg_ingress_self
#   #egress  = local.sg_egress_self

#   ingress {
#     cidr_blocks      = []
#     description      = "All - ICMPv4"
#     from_port        = -1
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "icmp"
#     security_groups  = []
#     self             = true
#     to_port          = -1
#   }
#   ingress {
#     cidr_blocks      = []
#     description      = "All - ICMPv6"
#     from_port        = -1
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "icmpv6"
#     security_groups  = []
#     self             = true
#     to_port          = -1
#   }
#   ingress {
#     cidr_blocks      = []
#     description      = "HTTP - TCP"
#     from_port        = 80
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "tcp"
#     security_groups  = []
#     self             = true
#     to_port          = 80
#   }
#   ingress {
#     cidr_blocks      = []
#     description      = "HTTPS - TCP"
#     from_port        = 443
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "tcp"
#     security_groups  = []
#     self             = true
#     to_port          = 443
#   }

#   egress {
#     cidr_blocks      = []
#     description      = "Self - ICMPv4"
#     from_port        = -1
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "icmp"
#     security_groups  = []
#     self             = true
#     to_port          = -1
#   }
#   egress {
#     cidr_blocks      = []
#     description      = "Selft - ICMPv6"
#     from_port        = -1
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "icmpv6"
#     security_groups  = []
#     self             = true
#     to_port          = -1
#   }
#   egress {
#     cidr_blocks      = []
#     description      = "Self - All"
#     from_port        = 0
#     ipv6_cidr_blocks = []
#     prefix_list_ids  = []
#     protocol         = "-1"
#     security_groups  = []
#     self             = true
#     to_port          = 0
#   }

#   tags = merge(local.tags,
#     tomap(
#       {
#         "Name" = "${local.name_prefix}-self-0"
#       }
#   ))
# }