﻿using System.Threading.Tasks;

namespace sfa.Tl.Marketing.Communication.SearchPipeline;

public interface ISearchStep
{
    Task Execute(ISearchContext searchContext);
}