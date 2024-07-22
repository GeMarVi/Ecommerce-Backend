using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace Ecommerce.Shared.Validations
{
    public class IsBoolValidationsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            if (value is bool)
            {
                return true;
            }

            string stringValue = value.ToString();
            if (bool.TryParse(stringValue, out bool result))
            {
                return true;
            }

            ErrorMessage = "Campo solo acepta true o false";
            return false;
        }
    }

    public class AllowedExtensionAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtension;

        public AllowedExtensionAttribute(string[] extension)
        {
            _allowedExtension = extension;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as List<IFormFile>;

            if (file != null)
            {
                foreach (var item in file)
                {
                    var extension = Path.GetExtension(item.FileName);
                    if (!_allowedExtension.Contains(extension.ToLower()))
                    {
                        return new ValidationResult("Solo se permiten extensiones JPG JPEG PNG");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }

    public class MaxFileResolutionAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileResolutionAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var files = value as List<IFormFile>;

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > _maxFileSize * 1024 * 1024)
                    {
                        return new ValidationResult("Tamaño maximo de cada imagen 2 mb");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }

    public class MaxImagesLongAttribute : ValidationAttribute
    {
        private readonly int _maxImages;

        public MaxImagesLongAttribute(int maxImages)
        {
            _maxImages = maxImages;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var files = value as List<IFormFile>;

            if (files != null)
            {
                if (files.Count > _maxImages)
                {
                    return new ValidationResult($"Solo se permiten máximo {_maxImages} imagenes por producto");
                }
            }
            return ValidationResult.Success;
        }
    }

    //This class validates that the size values ​​are valid as well as that they correspond to their corresponding stock
    public static class SizeStockValidation
    {
        public static bool IsCorrect(List<double> sizes)
        {
            foreach (var item in sizes)
            {
                string sizeAsString = item.ToString();

                if (!Regex.IsMatch(sizeAsString, @"^(2(\.5)?|[3-9](\.5)?|10(\.5)?|11(\.5)?|12(\.5)?|13)$"))
                {
                    return false;
                }
            }
            return true;
        }

        //if lists have data
        public static bool IsCorrect(List<int> stock)
        {
            foreach (var item in stock)
            {
                if (item <= 0)
                {
                    return false;
                }
            }

            return true;
        }
        //Each stock list and sizes are related to each other, the first value of one corresponds to the value of the other, therefore they must have the same length
        public static bool IsCorrect(List<int> stock, List<double> sizes)
        {
            if (stock.Count() == sizes.Count())
            {
                return true;
            }

            return false;
        }
    }

    public class ConstantProductValuesAttribute : ValidationAttribute
    {
        private readonly string _field;
        public ConstantProductValuesAttribute(string field)
        {
            _field = field;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var allowedValues = GetAllowedValues();

            if (!allowedValues.Contains(value))
            {
                return new ValidationResult($"Los valores aceptados son {string.Join(", ", allowedValues)}");
            }

            return ValidationResult.Success;
        }
        private HashSet<string> GetAllowedValues()
        {
            return _field switch
            {
                "Gender" => new HashSet<string>() { "Caballeros", "Damas", "Niños", "Unisex" },
                "Brand" => new HashSet<string>() { "Nike", "Adidas", "Reebook", "Puma", "Jordan", "Under Armour" },
                "ClosureType" => new HashSet<string>() { "Velcro", "Cordon Elastico", "Elastico", "Cierre", "Cordon" },
                "OuterMaterial" => new HashSet<string>() { "Caucho", "Tela", "Cuero", "Nailon", "Plastico", "GoreTex", "Poliéster", "Piel Sintetica" },
                "SoleMaterial" => new HashSet<string>() { "Piel", "Caucho", "Goma", "Crepe", "Espartano", "TF" },
                "TypeDeport" => new HashSet<string>() { "Running", "Basketball", "Crossfit", "Casual", "Entrenamiento", "Futbol", "Senderismo", "Sandalias" },
                "ProductStatus" => new HashSet<string>() { "Nuevo Lanzamiento", "Ultimas Piezas", "Mas comprados", "Normal" },
                _ => new HashSet<string>()
            };
        }
    }
}
