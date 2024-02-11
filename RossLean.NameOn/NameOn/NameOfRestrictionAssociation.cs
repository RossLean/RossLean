using Garyon.Extensions;
using RoseLynn;
using RossLean.NameOn.Core;
using System.Collections.Generic;

namespace RossLean.NameOn;

using static IdentifiableSymbolKind;

public class NameOfRestrictionAssociation
{
    private readonly RestrictionsDictionary dictionary = new();

    public NameOfRestrictions RestrictionForAllKinds => this[All];

    public void AddKinds(IdentifiableSymbolKind appliedKinds, NameOfRestrictions restrictions)
    {
        dictionary.AddKinds(appliedKinds, restrictions);
    }

    public NameOfRestrictions this[IdentifiableSymbolKind distinctKind] => dictionary.ValueOrDefault(distinctKind);

    private sealed class RestrictionsDictionary : Dictionary<IdentifiableSymbolKind, NameOfRestrictions>
    {
        // Not overengineering this today
        public static readonly IdentifiableSymbolKind[] DistinctKinds =
        [
            Namespace,

            // Types
            Class,
            Struct,
            Interface,
            Delegate,
            Enum,

            RecordClass,
            RecordStruct,

            // Parameters
            Parameter,
            GenericParameter,

            // Members
            Field,
            Property,
            Event,
            Method,
        ];
        private static readonly IdentifiableSymbolKind[] globalFlags =
        [
            None,
            Alias,
        ];

        private static readonly RestrictionsDictionary template;

        static RestrictionsDictionary()
        {
            template = new(DistinctKinds.Length * globalFlags.Length + 1);

            template.Add(All, default);
            foreach (var flag in globalFlags)
                foreach (var kind in DistinctKinds)
                    template.Add(kind | flag, default);
        }

        private RestrictionsDictionary(int capacity)
            : base(capacity) { }

        public RestrictionsDictionary()
            : base(template) { }

        public void AddKinds(IdentifiableSymbolKind appliedKinds, NameOfRestrictions restrictions)
        {
            foreach (var distinct in Keys)
            {
                if ((appliedKinds & distinct) != distinct)
                    continue;

                this[distinct] = restrictions;
            }
        }
    }
}
