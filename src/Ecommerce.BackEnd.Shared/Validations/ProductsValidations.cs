namespace Ecommerce.BackEnd.Shared.Validations
{
    namespace Ecommerce.BackEnd.Shared.Constants
    {
        public static class ProductFieldAllowedValues
        {
            private static readonly Dictionary<string, HashSet<string>> _values = new()
            {
                ["Gender"] = new() { "Caballeros", "Damas", "Niños", "Unisex" },
                ["Brand"] = new() { "Nike", "Adidas", "Reebook", "Puma", "Jordan", "Under Armour" },
                ["ClosureType"] = new() { "Velcro", "Cordon Elastico", "Elastico", "Cierre", "Cordon" },
                ["OuterMaterial"] = new() { "Caucho", "Tela", "Cuero", "Nailon", "Plastico", "GoreTex", "Poliéster", "Piel Sintetica" },
                ["SoleMaterial"] = new() { "Piel", "Caucho", "Goma", "Crepe", "Espartano", "TF" },
                ["TypeDeport"] = new() { "Running", "Basketball", "Crossfit", "Casual", "Entrenamiento", "Futbol", "Senderismo", "Sandalias" },
                ["ProductStatus"] = new() { "Nuevo Lanzamiento", "Ultimas Piezas", "Mas comprados", "Normal" }
            };

            public static HashSet<string> Get(string field)
            {
                return _values.TryGetValue(field, out var values)
                    ? values
                    : new HashSet<string>();
            }

            public static IReadOnlyCollection<string> AllowedFieldsSearchable => new[]
            {
                "Gender",
                "Brand",
                "TypeDeport"
            };

            public static IReadOnlyCollection<string> AllowedFields => _values.Keys;
        }
    }
}
