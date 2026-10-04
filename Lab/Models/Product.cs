using System.ComponentModel.DataAnnotations;

namespace MyAppMVC.Models;

public class Product : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm bắt buộc nhập.")]
    [StringLength(150, MinimumLength = 6, ErrorMessage = "Tên phải có ít nhất 6 ký tự và nhiều nhất 150 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ảnh sản phẩm bắt buộc chọn.")]
    public string Image { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giá sản phẩm bắt buộc nhập.")]
    [Range(typeof(float), "100000", "3.4028235E+38", ErrorMessage = "Giá sản phẩm phải ít nhất là 100000.")]
    public float Price { get; set; }

    [Required(ErrorMessage = "Giá khuyến mãi bắt buộc nhập.")]
    [Range(typeof(float), "0", "3.4028235E+38", ErrorMessage = "Giá khuyến mãi không được âm.")]
    public float SalePrice { get; set; }

    [Required(ErrorMessage = "Mô tả bắt buộc nhập.")]
    [StringLength(1500, ErrorMessage = "Mô tả không được vượt quá 1500 ký tự.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Danh mục bắt buộc chọn.")]
    public int? CategoryId { get; set; }

    public Category? Category { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Price >= 100000 && SalePrice >= Price * 0.9f)
        {
            yield return new ValidationResult(
                "Giá khuyến mãi phải thấp hơn giá chuẩn ít nhất 10%.",
                new[] { nameof(SalePrice) });
        }
    }
}
