using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCoreTheHive.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("username")]
        public string Username { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("password")]
        public byte[] Password { get; set; }

        [Column("salt")]
        public string Salt { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("foto_perfil")]
        public string FotoPerfil { get; set; }

        [ForeignKey("Rol")]
        public int Rol { get; set; }
    }
}
