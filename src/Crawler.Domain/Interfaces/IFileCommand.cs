using Crawler.Domain.Entities;

namespace Crawler.Domain.Interfaces;

public interface IFileCommand
{
    HtmlWithImages Execute();
}