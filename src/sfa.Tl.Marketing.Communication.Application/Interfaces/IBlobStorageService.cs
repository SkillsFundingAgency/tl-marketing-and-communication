﻿using System.IO;
using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.Application.Interfaces;

public interface IBlobStorageService
{
    Task Upload(Stream stream,
        string containerName,
        string fileName,
        string contentType);

    Task<Stream> Get(
        string containerName,
        string fileName);
}