using System.ComponentModel.DataAnnotations;

namespace MyAppMVC.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên danh mục bắt buộc nhập.")]
    [StringLength(150, MinimumLength = 6, ErrorMessage = "Tên phải có ít nhất 6 ký tự và nhiều nhất 150 ký tự.")]
    public string Name { get; set; } = string.Empty;
}
