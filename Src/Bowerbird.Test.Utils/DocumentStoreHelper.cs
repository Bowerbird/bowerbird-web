﻿using Raven.Client;
using Raven.Client.Embedded;

namespace Bowerbird.Test.Utils
{
    public class DocumentStoreHelper
    {
     
        public static IDocumentStore TestDocumentStore()
        {
            return new EmbeddableDocumentStore()
            {
                RunInMemory = true
            }
            .Initialize();

        }

    }
}