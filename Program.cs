
namespace Kaspa
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthorization();
            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();
      


            app.MapGet("/", () =>
            {
                return Results.File("index.html");

            });


            app.MapGet("/Moviespin", () =>
            {

                MyProgram.RunAsync();

                return Results.Ok();
            });

            app.Run();
        }
    }
}
