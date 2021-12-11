using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HiraBots.Editor
{
    /// <summary>
    /// Helpers to cook objects into the build or before play mode.
    /// </summary>
    internal static class CookingHelpers
    {
        /// <summary>
        /// Validate all the runtime-interpreted blackboard templates in the project, and pack them into a collection.
        /// </summary>
        internal static bool TryGenerateInterpretedBlackboardTemplateCollection(out BlackboardTemplateCollection output)
        {
            var validator = new BlackboardTemplateValidator();

            // objects used for testing are subclasses that get conditionally compiled, leaving the asset itself as
            // not being determined a Blackboard Template

            // the templates get ordered by their hierarchy index because the runtime compiler assumes they're
            // ordered in a way that they can be compiled in the sequence

            var templatesToCook = AssetDatabase
                .FindAssets($"t:{typeof(BlackboardTemplate).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<BlackboardTemplate>)
                .Where(bt => bt.backends.HasFlag(BackendType.RuntimeInterpreter))
                .OrderBy(bt => bt.hierarchyIndex);

            var validatedTemplates = new List<BlackboardTemplate>();

            var result = true;

            foreach (var template in templatesToCook)
            {
                if (validator.Validate(template, out var errorText))
                {
                    validatedTemplates.Add(template);
                    continue;
                }

                Debug.LogError(errorText, template);
                result = false;
            }

            output = BlackboardTemplateCollection.Create(validatedTemplates.ToArray());
            return result;
        }

        /// <summary>
        /// Validate all the runtime-interpreted LGOAP domains in the project, and pack them into a collection.
        /// </summary>
        internal static bool TryGenerateInterpretedLGOAPDomainCollection(
            ReadOnlyDictionaryAccessor<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>> blackboardData,
            out LGOAPDomainCollection output)
        {
            var validator = new LGOAPDomainValidator(blackboardData);

            // objects used for testing are subclasses that get conditionally compiled, leaving the asset itself as
            // not being determined a Blackboard Template

            var domainsToCook = AssetDatabase
                .FindAssets($"t:{typeof(LGOAPDomain).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<LGOAPDomain>)
                .Where(d => d.backends.HasFlag(BackendType.RuntimeInterpreter));

            var validatedDomains = new List<LGOAPDomain>();

            var result = true;

            foreach (var domain in domainsToCook)
            {
                if (validator.Validate(domain, out var errorText))
                {
                    validatedDomains.Add(domain);
                    continue;
                }

                Debug.LogError(errorText, domain);
                result = false;
            }

            output = LGOAPDomainCollection.Create(validatedDomains.ToArray());
            return result;
        }

        /// <summary>
        /// Validate all the blackboard templates to generate code for, and pack them into an array.
        /// </summary>
        internal static bool TryGetBlackboardTemplatesToGenerateCodeFor(out (string path, BlackboardTemplate template)[] output)
        {
            var validator = new BlackboardTemplateValidator();

            // no need to order the templates by hierarchy indices here, because it does not matter

            var templatesToGenerateCodeFor = AssetDatabase
                .FindAssets($"t:{typeof(BlackboardTemplate).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => (System.IO.Path.ChangeExtension(path, "cs"), AssetDatabase.LoadAssetAtPath<BlackboardTemplate>(path)))
                .Where(tuple => tuple.Item2.backends.HasFlag(BackendType.CodeGenerator));

            var result = true;

            var validatedTemplatesToGenerateCodeFor = new List<(string path, BlackboardTemplate template)>();

            foreach (var (path, template) in templatesToGenerateCodeFor)
            {
                if (validator.Validate(template, out var errorText))
                {
                    validatedTemplatesToGenerateCodeFor.Add((path, template));
                    continue;
                }

                Debug.LogError(errorText, template);
                result = false;
            }

            output = validatedTemplatesToGenerateCodeFor.ToArray();
            return result;
        }

        /// <summary>
        /// Validate all the LGOAP domains to generate code for, and pack them into an array.
        /// </summary>
        internal static bool TryGetLGOAPDomainsToGenerateCodeFor(
            ReadOnlyDictionaryAccessor<BlackboardTemplate, ReadOnlyHashSetAccessor<BlackboardKey>> blackboardData,
            out (string path, LGOAPDomain domain)[] output)
        {
            var validator = new LGOAPDomainValidator(blackboardData);

            var domainsToGenerateCodeFor = AssetDatabase
                .FindAssets($"t:{typeof(LGOAPDomain).FullName}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => (System.IO.Path.ChangeExtension(path, "cs"), AssetDatabase.LoadAssetAtPath<LGOAPDomain>(path)))
                .Where(tuple => tuple.Item2.backends.HasFlag(BackendType.CodeGenerator));

            var result = true;

            var validatedTemplatesToGenerateCodeFor = new List<(string path, LGOAPDomain domain)>();

            foreach (var (path, template) in domainsToGenerateCodeFor)
            {
                if (validator.Validate(template, out var errorText))
                {
                    validatedTemplatesToGenerateCodeFor.Add((path, template));
                    continue;
                }

                Debug.LogError(errorText, template);
                result = false;
            }

            output = validatedTemplatesToGenerateCodeFor.ToArray();
            return result;
        }
    }
}