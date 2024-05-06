using Microsoft.EntityFrameworkCore;
using ApiCoreTheHive.Data;
using ApiCoreTheHive.Models;
using ApiCoreTheHive.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ApiCoreTheHive.Repositories
{
    public class RepositoryTheHive
    {

        private TheHiveContext context;

        public RepositoryTheHive(TheHiveContext context)
        {
            this.context = context;
        }


        #region USUARIOS

        public Usuario GetUser(string username)
        {
            var usuario = (from u in context.Usuarios
                           where u.Username == username
                           select u).FirstOrDefault();

            return usuario;
        }

        public List<Usuario> GetUsuarios()
        {
            var consulta = from datos in context.Usuarios
                           select datos;
            return consulta.ToList();
        }

        public async Task<Usuario> RegisterUserAsync(string username, string password, string email, string nombre)
        {
            Usuario user = new Usuario();

            user.Username = username;
            user.Nombre = nombre;
            user.Email = email;
            user.Descripcion = "";
            user.FotoPerfil = "";
            user.Telefono = "";
            user.Rol = 2;


            user.Salt = HelperCryptography.GenerateSalt();
            user.Password =
                HelperCryptography.EncryptPassword(password, user.Salt);

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUser(Usuario usuario)
        {
            var user = context.Usuarios.FirstOrDefault(u => u.Username == usuario.Username);

            if (user != null)
            {
                user.Nombre = string.IsNullOrEmpty(usuario.Nombre) ? string.Empty : usuario.Nombre;
                user.Email = string.IsNullOrEmpty(usuario.Email) ? string.Empty : usuario.Email;
                user.Telefono = string.IsNullOrEmpty(usuario.Telefono) ? string.Empty : usuario.Telefono;
                user.Descripcion = string.IsNullOrEmpty(usuario.Descripcion) ? string.Empty : usuario.Descripcion;


                if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                {
                    user.FotoPerfil = usuario.FotoPerfil;

                    var sql = $"UPDATE Publicaciones SET foto_perfil = '{usuario.FotoPerfil}' WHERE username = '{usuario.Username}'";

                    // Ejecuta el comando SQL
                    await context.Database.ExecuteSqlRawAsync(sql);

                }


                context.Entry(user).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Usuario no encontrado");
            }
        }

        public async Task DeleteUser(string username)
        {
            var user = await context.Usuarios.FindAsync(username);

            // Eliminar Likes asociados a las publicaciones del usuario
            var deleteLikesSql = $"DELETE FROM Likes WHERE username = '{username}'";
            await context.Database.ExecuteSqlRawAsync(deleteLikesSql);

            // Eliminar Guardados asociados a las publicaciones del usuario
            var deleteGuardadosSql = $"DELETE FROM Guardados WHERE username = '{username}'";
            await context.Database.ExecuteSqlRawAsync(deleteGuardadosSql);

            // Eliminar Seguidores del usuario
            var deleteSeguidoresSql = $"DELETE FROM Seguidores WHERE seguido_username = '{username}' OR seguidor_username = '{username}'";
            await context.Database.ExecuteSqlRawAsync(deleteSeguidoresSql);

            // Eliminar Publicaciones del usuario
            var deletePublicacionesSql = $"DELETE FROM Publicaciones WHERE username = '{username}'";
            await context.Database.ExecuteSqlRawAsync(deletePublicacionesSql);

            // Finalmente, eliminar al usuario
            context.Usuarios.Remove(user);
            await context.SaveChangesAsync();
        }


        public bool UsuarioExists(string username)
        {
            var consulta = from u in context.Usuarios
                           where u.Username == username
                           select u;

            return consulta.Any();
        }

        public bool EmailExists(string email)
        {
            var consulta = from u in context.Usuarios
                           where u.Email == email
                           select u;

            return consulta.Any();
        }



        #endregion

        #region LOGIN

        public async Task<bool> LogInUserAsync(string username, string password)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);

            if (usuario != null)
            {
                string salt = usuario.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passUser = usuario.Password;
                bool response = HelperCryptography.CompareArrays(temp, passUser);

                return response;
            }
            else
            {
                return false;
            }
        }

        public async Task<Usuario> LogInSeguro(string username, string password)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);

            if (usuario != null)
            {
                string salt = usuario.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(password, salt);
                byte[] passUser = usuario.Password;
                bool response = HelperCryptography.CompareArrays(temp, passUser);

                if (response)
                {
                    return usuario;
                }
            }
            return null;
        }

        #endregion

        #region PUBLICACIONES

        public List<Publicacion> GetAllPublicaciones()
        {
            return context.Publicaciones
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();
        }

        public List<Publicacion> GetPublicacionesUsuario(string username)
        {
            return context.Publicaciones
                .Where(p => p.Username == username)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();
        }

        public bool PublicacionExists(int idPublicacion)
        {
            var consulta = from p in context.Publicaciones
                           where p.IdPublicacion == idPublicacion
                           select p;

            return consulta.Any();
        }

        public List<Publicacion> GetAllPublicacionesExceptoUsuario(string username)
        {
            return context.Publicaciones
                .Where(p => p.Username != username)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();
        }


        public List<Publicacion> GetAllPublicacionesSeguidosExceptoUsuario(string username)
        {
            var seguidos = context.Seguidores
                .Where(s => s.SeguidorUsername == username)
                .Select(s => s.SeguidoUsername)
                .ToList();

            var publicacionesSeguidos = context.Publicaciones
                .Where(p => seguidos.Contains(p.Username) && p.Username != username)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();

            return publicacionesSeguidos;
        }

        public async Task AddPublicacion(Publicacion publicacion)
        {
            if (publicacion == null || publicacion.Texto == null || publicacion.FechaPublicacion == null || publicacion.Username == null)
            {
                throw new ArgumentNullException("Alguno de los campos de la publicación es nulo.");
            }

            context.Publicaciones.Add(publicacion);

            await context.SaveChangesAsync();
        }
        public int GetNextPublicacionId()
        {

            int maxId = context.Publicaciones.Max(p => (int?)p.IdPublicacion) ?? 0;
            return maxId + 1;
        }

        
        public void EliminarPublicacion(int idPublicacion, string username)
        {
 
            Publicacion publicacion = context.Publicaciones.FirstOrDefault(p => p.IdPublicacion == idPublicacion && p.Username == username);

            if (publicacion != null)
            {
                var likes = context.Likes.Where(l => l.IdPublicacion == idPublicacion);
                context.Likes.RemoveRange(likes);

                context.Publicaciones.Remove(publicacion);
                context.SaveChanges();
            }
        }



        #endregion

        #region LIKES

        public int GetLikes(int idPublicacion)
        {
            return context.Likes.Count(l => l.IdPublicacion == idPublicacion);
        }


        public bool IsLiked(int idPublicacion, string username)
        {
            return context.Likes.Any(l => l.IdPublicacion == idPublicacion && l.Username == username);
        }

        public void Like(int idPublicacion, string username)
        {
            if (!IsLiked(idPublicacion, username))
            {
                Like like = new Like { IdPublicacion = idPublicacion, Username = username };
                context.Likes.Add(like);
                context.SaveChanges();
            }
        }


        public void Dislike(int idPublicacion, string username)
        {
            Like like = context.Likes.FirstOrDefault(l => l.IdPublicacion == idPublicacion && l.Username == username);
            if (like != null)
            {
                context.Likes.Remove(like);
                context.SaveChanges();
            }
        }

        #endregion

        #region SEGUIDORES

        public int GetSeguidosCount(string username)
        {
            return context.Seguidores.Count(s => s.SeguidorUsername == username);
        }

        public int GetSeguidoresCount(string username)
        {
            return context.Seguidores.Count(s => s.SeguidoUsername == username);
        }


        public bool IsFollowing(string seguidorUsername, string seguidoUsername)
        {
            return context.Seguidores.Any(s => s.SeguidorUsername == seguidorUsername && s.SeguidoUsername == seguidoUsername);
        }


        public void Follow(string seguidorUsername, string seguidoUsername)
        {

            if (!IsFollowing(seguidorUsername, seguidoUsername))
            {
            
                Seguidores seguimiento = new Seguidores
                {
                    SeguidorUsername = seguidorUsername,
                    SeguidoUsername = seguidoUsername
                };

                context.Seguidores.Add(seguimiento);
                context.SaveChanges();
            }
        }

        public void Unfollow(string seguidorUsername, string seguidoUsername)
        {

            Seguidores seguimiento = context.Seguidores.FirstOrDefault(s => s.SeguidorUsername == seguidorUsername && s.SeguidoUsername == seguidoUsername);

 
            if (seguimiento != null)
            {
     
                context.Seguidores.Remove(seguimiento);
                context.SaveChanges();
            }
        }


        public List<Usuario> GetSeguidores(string username)
        {
            var seguidores = (from s in context.Seguidores
                              join u in context.Usuarios on s.SeguidorUsername equals u.Username
                              where s.SeguidoUsername == username
                              select new Usuario
                              {
                                  Username = u.Username,
                                  FotoPerfil = u.FotoPerfil
                              }).ToList();

            return seguidores;
        }

        public List<Usuario> GetSeguidos(string username)
        {
            var seguidos = (from s in context.Seguidores
                            join u in context.Usuarios on s.SeguidoUsername equals u.Username
                            where s.SeguidorUsername == username
                            select new Usuario
                            {
                                Username = u.Username,
                                FotoPerfil = u.FotoPerfil
                            }).ToList();

            return seguidos;
        }

        #endregion

        #region BUSCADOR

        public List<Usuario> BuscarUsuarios(string query)
        {
            var usuariosEncontrados = context.Usuarios
                .Where(u => u.Nombre.Contains(query) || u.Username.Contains(query))
                .Select(u => new Usuario { Username = u.Username, FotoPerfil = u.FotoPerfil })
                .ToList();

            return usuariosEncontrados;
        }

        #endregion







    }
}
