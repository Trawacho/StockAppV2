﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace StockApp.Lib;

public interface IPaginator
{
    Task<List<UIElement>> PaginateAsync(Func<UIElement> pageFactory, Size pageSize, Thickness pageMargins, CancellationToken cancellationToken);
    
    Task<List<UIElement>> PaginateAsync(Func<UIElement> pageFactory, Func<UIElement> tableHeaderFactory, Size pageSize, Thickness pageMargins, CancellationToken cancellationToken);

    FixedDocument GetFixedDocumentFromPages(List<UIElement> uiElements, Size pageSize);
}
