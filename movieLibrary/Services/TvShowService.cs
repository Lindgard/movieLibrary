using movieLibrary.Models;

namespace movieLibrary.Services;

public class TvShowService
{
    private List<string> tvList = [];

    public TvShowService()
    {
        tvList.Add = TvShow.Title("X-Files");
    }
}