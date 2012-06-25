using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Extensions
{
    public static class DomainExtensions
    {
        public static string MinifyId<T>(this string id) where T : DomainModel
        {
            string modelType = shortTypeName(typeof(T).ToString().ToLower());

            return MutatedId(modelType, id, true);
        }

        public static string VerbosifyId<T>(this string id) where T: DomainModel
        {
            string modelType = typeof(T).ToString().ToLower();

            return MutatedId(modelType, id, false);
        }

        private static string shortTypeName(string fullyQualifiedTypeName)
        {
            var tokens = fullyQualifiedTypeName.Split('.');

            return tokens[tokens.Length - 1];
        }

        private static string MutatedId(string modelType, string id, bool stripTypeParameter)
        {
            string mutatedId = string.Empty;

            switch (modelType)
            {
                case "user":
                    {
                        if (stripTypeParameter) mutatedId = MinifiedId("users", id);
                        else mutatedId = VerbosifiedId("users", id);
                    }
                    break;
                case "project":
                    {
                        if (stripTypeParameter) mutatedId = MinifiedId("projects", id);
                        else mutatedId = VerbosifiedId("projects", id);
                    }
                    break;
                case "team":
                    {
                        if (stripTypeParameter) mutatedId = MinifiedId("teams", id);
                        else mutatedId = VerbosifiedId("teams", id);
                    }
                    break;
                case "organisation":
                    {
                        if (stripTypeParameter) mutatedId = MinifiedId("organisations", id);
                        else mutatedId = VerbosifiedId("organisations", id);
                    }
                    break;
                case "observation":
                    {
                        if (stripTypeParameter) mutatedId = MinifiedId("observations", id);
                        else mutatedId = VerbosifiedId("observations", id);
                    }
                    break;
                case "post":
                    {
                        if (stripTypeParameter) mutatedId = MinifiedId("posts", id);
                        else mutatedId = VerbosifiedId("posts", id);
                    }
                    break;
                default:
                    { }
                    break;
            }

            return mutatedId;
        }

        private static string MinifiedId(string prefix, string id)
        {
            return id.StartsWith("{0}/".AppendWith(prefix)) ? id.Replace("{0}/".AppendWith(prefix), "") : id;
        }

        private static string VerbosifiedId(string prefix, string id)
        {
            return id.StartsWith("{0}/".AppendWith(prefix)) ? id : "{0}/{1}".AppendWith(prefix, id);        
        }
    }
}