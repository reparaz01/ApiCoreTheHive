using ApiCoreTheHive.Models;
using ApiCoreTheHive.Helpers;
using ApiCoreTheHive.Models;
using ApiCoreTheHive.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiCoreTheHive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private RepositoryTheHive repo;
        //CUANDO GENEREMOS EL TOKEN, DEBEMOS INTEGRAR
        //DENTRO DE DICHO TOKEN, ISSUER, AUDIENCE...
        //PARA QUE LO VALIDE CUANDO NOS LO ENVIEN.
        private HelperActionServicesOAuth helper;

        public OAuthController(RepositoryTheHive repo
            , HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        //NECESITAMOS UN METODO POST PARA VALIDAR EL 
        //USUARIO Y QUE RECIBIRA LoginModel
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            //BUSCAMOS AL EMPLEADO EN NUESTRO REPO
            Usuario usuario =
                await this.repo.LogInSeguro(model.Username, model.Password);
            if (usuario == null)
            {
                return Unauthorized();
            }
            else
            {
                //DEBEMOS CREAR UNAS CREDENCIALES PARA 
                //INCLUIRLAS DENTRO DEL TOKEN Y QUE ESTARAN 
                //COMPUESTAS POR EL SECRET KEY CIFRADO Y EL TIPO
                //DE CIFRADO QUE DESEEMOS INCLUIR EN EL TOKEN
                SigningCredentials credentials =
                    new SigningCredentials(
                        this.helper.GetKeyToken()
                        , SecurityAlgorithms.HmacSha256);

                //CONVERTIMOS EL EMPLEADO A FORMATO JSON
                string jsonEmpleado =
                    JsonConvert.SerializeObject(usuario);
                //ESTO DEBE IR CIFRADO POR NOSOTROS

                //CREAMOS UN ARRAY DE CLAIMS CON TODA 
                //LA INFORMACION QUE DESEAMOS GUARDAR EN EL TOKEN
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonEmpleado)
                };

                //EL TOKEN SE GENERA CON UNA CLASE Y 
                //DEBEMOS INDICAR LOS ELEMENTOS QUE ALMACENARA 
                //DENTRO DE DICHO TOKEN, POR EJEMPLO, ISSUER,
                //AUDIENCE O EL TIEMPO DE VALIDACION DEL TOKEN
                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: informacion,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );
                //POR ULTIMO, DEVOLVEMOS UNA RESPUESTA AFIRMATIVA
                //CON UN OBJETO ANONIMO EN FORMATO JSON
                return Ok(
                    new
                    {
                        response =
                        new JwtSecurityTokenHandler()
                        .WriteToken(token)
                    });
            }
        }
    }
}