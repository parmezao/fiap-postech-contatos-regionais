using System.ComponentModel.DataAnnotations;

namespace ContatosRegionais.Application.DTO;

public class ContatoDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Campo Nome é obrigatório!")]
    [MaxLength(100, ErrorMessage = "Limite máximo atingido! Máximo de 100 caracteres")]
    public required string Nome { get; set; }

    [Required(ErrorMessage = "Campo Email é obrigatório!")]
    [EmailAddress(ErrorMessage = "Email inválido!")]
    [MaxLength(60, ErrorMessage = "Limite máximo atingido! Máximo de 60 caracteres")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Campo Telefone é obrigatório!")]
    [MaxLength(10, ErrorMessage = "Limite máximo atingido! Máximo de 10 caracteres")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "Campo Telefone aceita somente números!")]
    public required string Telefone { get; set; }

    [Required(ErrorMessage = "Campo DDD é obrigatório!")]
    [Range(10, 99, ErrorMessage = "Mínimo e Máximo de 2 caracteres!")]
    [RegularExpression("^[0-9]*$", ErrorMessage = "Campo DDD aceita somente números!")]
    public int Ddd { get; set; }
}
