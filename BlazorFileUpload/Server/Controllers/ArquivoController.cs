using BlazorFileUpload.Server.Data;
using BlazorFileUpload.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BlazorFileUpload.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArquivoController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;

        public ArquivoController(IWebHostEnvironment env, DataContext context)
        {
            _env = env;
            _context = context;
        }

        //Task serve para indicar que a ação a ser executada é assíncrona.

        //ActionResult serve para indicar o tipo de objeto/resultado que a função retornará.
        //Nesse caso, retornará uma lista de ResultadoUpload.

        [HttpGet]
        
        public async Task<ActionResult<List<ResultadoUpload>>> ListarArquivos()
        {
            //List<ResultadoUpload> listaArquivos = new List<ResultadoUpload>();
            //var listaArquivos = await _context.Uploads.FirstOrDefaultAsync();
            //var path = Path.Combine(_env.ContentRootPath, "uploads");
            return Ok(await _context.Uploads.ToListAsync());
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var uploadResult = await _context.Uploads.FirstOrDefaultAsync(u => u.StoredFileName.Equals(fileName));

            var path = Path.Combine(_env.ContentRootPath, "uploads", fileName);

            var memory = new MemoryStream();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, uploadResult.ContentType, Path.GetFileName(path));
        }

        [HttpPost]
        public async Task<ActionResult<List<ResultadoUpload>>> UploadFile(List<IFormFile> files)
        {
            List<ResultadoUpload> resultadoUploads = new List<ResultadoUpload>();

            foreach(var file in files)
            {
                var uploadResult = new ResultadoUpload();
                string nomeDeArquivo_Confiavel_Para_FileStorage;
                var nomeDeArquivo_Nao_Confiavel = file.FileName;
                uploadResult.FileName = nomeDeArquivo_Nao_Confiavel;
                //var nomeDeArquivo_Confiavel_ForDisplay = WebUtility.HtmlEncode(nomeDeArquivo_Nao_Confiavel);

                nomeDeArquivo_Confiavel_Para_FileStorage = Path.GetRandomFileName();
                var path = Path.Combine(_env.ContentRootPath, "uploads", nomeDeArquivo_Confiavel_Para_FileStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.StoredFileName = nomeDeArquivo_Confiavel_Para_FileStorage;
                uploadResult.ContentType = file.ContentType;
                resultadoUploads.Add(uploadResult);

                _context.Uploads.Add(uploadResult);
            }

            await _context.SaveChangesAsync();

            return Ok(resultadoUploads);
        }
    }
}
