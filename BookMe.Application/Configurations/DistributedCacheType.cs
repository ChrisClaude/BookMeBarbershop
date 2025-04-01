using System;
using System.Runtime.Serialization;

namespace BookMe.Application.Configurations;

public enum DistributedCacheType
{
  [EnumMember(Value = "memory")]
  Memory,
  [EnumMember(Value = "sqlserver")]
  SqlServer,
  [EnumMember(Value = "redis")]
  Redis,
  [EnumMember(Value = "redissynchronizedmemory")]
  RedisSynchronizedMemory
}
