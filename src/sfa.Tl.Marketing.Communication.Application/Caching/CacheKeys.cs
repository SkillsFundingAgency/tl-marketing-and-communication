﻿using System;

namespace sfa.Tl.Marketing.Communication.Application.Caching;

public static class CacheKeys
{
    public const string ProviderTableDataKey = "PROVIDER_TABLE_DATA";
    public const string QualificationTableDataKey = "QUALIFICATION_TABLE_DATA";

    public static string PostcodeKey(string postcode)
    {
        if (postcode is null)
            throw new ArgumentNullException(nameof(postcode));

        if (string.IsNullOrWhiteSpace(postcode))
            throw new ArgumentException("A non-empty postcode is required", nameof(postcode));

        return $"POSTCODE__{postcode.Replace(" ", "").ToUpper()}";
    }

    public static string TownPartitionKey(string partitionKey)
    {
        if (partitionKey is null)
            throw new ArgumentNullException(nameof(partitionKey));

        if (string.IsNullOrWhiteSpace(partitionKey))
            throw new ArgumentException("A non-empty partition key is required", nameof(partitionKey));

        return $"TOWNS__{partitionKey.ToUpper()}";
    }
}