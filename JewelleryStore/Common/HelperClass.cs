using JewelleryStore.Entities;
using JewelleryStore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JewelleryStore.Common
{
    public static class HelperClass
    {
        public static void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                var user1 = new Entities.User
                {
                    UserId = 1,
                    Username = "Normal",
                    Password = "user@123",
                    IsPrivileged = false
                };

                context.Users.Add(user1);

                var user2 = new Entities.User
                {
                    UserId = 2,
                    Username = "Privileged",
                    Password = "user@123",
                    IsPrivileged = true
                };

                context.Users.Add(user2);

                context.SaveChanges();
            }
             
        }

        public static string GetAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(Constants.UserIdClaim, Convert.ToString(user.UserId)),
                new Claim(Constants.PrivilegedUserClaim, Convert.ToString(user.IsPrivileged))
            };

            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.Secret));

            var signingCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GetHTMLString(CalculatedPriceModel calculatedPriceModel)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>This is the generated PDF report of the gold price estimation</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Gold Price (per gram)</th>
                                        <th>Weight (grams)</th>
                                        <th>Discount %</th>
                                        <th>Total price</th>
                                    </tr>");

            sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                  </tr>", calculatedPriceModel.GoldPrice, calculatedPriceModel.Weight, calculatedPriceModel.Discount != null ? calculatedPriceModel.Discount.Value + " %" : "-", calculatedPriceModel.TotalPrice);

            sb.Append(@"
                                </table>
                            </body>
                        </html>");

            return sb.ToString();
        }
    }
}
