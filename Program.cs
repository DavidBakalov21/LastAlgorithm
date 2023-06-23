using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

string csvFilePath = "C:\\Users\\Давід\\RiderProjects\\Last4TermV2\\Last4TermV2\\movie_data.csv";
string csvFilePathUser = "C:\\Users\\Давід\\RiderProjects\\Last4TermV2\\Last4TermV2\\users_export.csv";
string csvFilePathRatings = "C:\\Users\\Давід\\RiderProjects\\Last4TermV2\\Last4TermV2\\ratings_export.csv";
var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    MissingFieldFound = null
};
var reader = new StreamReader(csvFilePath);
var csv = new CsvReader(reader, config);
var FilmList = new List<Film>();
var records = csv.GetRecords<Film>();

FilmList = records.Where(film => film.genres != null && (film.genres.Contains("Drama")|| 
                                                         film.genres.Contains("Comedy") || 
                                                         film.genres.Contains("Documentary")||
                                                         film.genres.Contains("Romance")||
                                                         film.genres.Contains("Action")||
                                                         film.genres.Contains("Adventure")||
                                                         film.genres.Contains("Fiction")||
                                                         film.genres.Contains("Animation")||
                                                         film.genres.Contains("Thriller")||
                                                         film.genres.Contains("Science Fiction")|| 
                                                         film.genres.Contains("Fantasy"))).ToList();


var dict = FilmList.ToDictionary(x => x.movie_id, x => x.genres);
var filmTitles = FilmList.Where(movies => !string.IsNullOrEmpty(movies.movie_title)).ToList();
var filmdict = FilmList.GroupBy(x => x.movie_title).ToDictionary(g => g.Key, g => g.Select(x => x.movie_id).ToList());

var readerRaiting = new StreamReader(csvFilePathRatings);
var csvRaiting = new CsvReader(readerRaiting, config);
var recordsRaitings = csvRaiting.GetRecords<Raiting>().ToList();



var readerUser = new StreamReader(csvFilePathUser);
var csvUSer = new CsvReader(readerUser, config);
var recordsUsers = csvUSer.GetRecords<Users>().ToList();
Console.WriteLine("d");
var Pointer = new Dictionary<string, int>()
{
    {"Drama",0},
    {"Comedy",1},
    {"Action",2},
    {"Adventure",2},
    {"Romance",3},
    {"Science Fiction",4},
    {"Fantasy",4},
    {"Animation",5},
    {"Thriller",6},
    {"Documentary",7}
};

Console.WriteLine("d");
var dictUSer = new Dictionary<string, List<Raiting>>();
foreach (var review in recordsRaitings)
{
    if (!dictUSer.ContainsKey(review.user_id))
    {
        dictUSer[review.user_id] = new List<Raiting>();
    }
    dictUSer[review.user_id].Add(review);
}

double[,] UserMatrix = new double[recordsUsers.Count, 8];
for (int i = 0; i < recordsUsers.Count(); i++)
{
    if (!dictUSer.ContainsKey(recordsUsers[i].username))
    {
        continue;
    }
    var Review = dictUSer[recordsUsers[i].username];
    var rates = new double[8];
    var ratesCount = new double[8];
    foreach (var VARIABLE in Review)
    {
        if (!dict.ContainsKey(VARIABLE.movie_id))
        {
            continue;
        }
        var genreString = dict[VARIABLE.movie_id].Replace("]", "").Replace("[", "").Replace("\"", "").Split(",");
        foreach (var genre in genreString)
        {
            if (Pointer.ContainsKey(genre))
            {
                rates[Pointer[genre]] += Convert.ToDouble(VARIABLE.rating_val);
                ratesCount[Pointer[genre]] += 1;
            }
        }
    }

    var genreScore = GenreScore(rates, ratesCount);
    int genreIndex = 0;

    for (int j = 0; j < UserMatrix.GetLength(1); j++)
    {
        if (genreIndex < genreScore.Length)
        {
            UserMatrix[i, j] = genreScore[genreIndex];
            genreIndex++;
        }
        else
        {
            break;
        }
    }

    
}
Console.WriteLine("d");
 double[] GenreScore(double[] rates, double[] ratesCount)
{
    for (int i = 0; i < rates.Length; i++)
    {
        rates[i] = rates[i] / ratesCount[i];
    }

    for (int i = 0; i < rates.Length; i++)
    {
        rates[i]=(rates[i] - rates.Min()) / (rates.Max() - rates.Min());
    }
    return rates;
}


var User = new double[8];
while (true)
{
    var input = Console.ReadLine().Split("*");
    if (input[0] == "rate")
    {
        var filmName = input[1];
        var rating = Convert.ToDouble(input[2]);
        
        if (filmdict.ContainsKey(filmName))
        {
            var filmId = filmdict[filmName];
            
            if (dict.ContainsKey(filmId[0]))
            {
                var genreList = dict[filmId[0]].Replace("]", "").Replace("[", "").Replace("\"", "").Split(",");
                
                foreach (var genre in genreList)
                {
                    if (Pointer.ContainsKey(genre))
                    {
                        if (User[Pointer[genre]] == 0)
                        {
                            User[Pointer[genre]] = rating;
                        }
                    }
                }

                Console.WriteLine($"You've rated a film '{filmName}' ({filmId}) as {rating}");
            }
        }
        else
        {
            var closest = LevinsteinList(filmName, filmTitles);
            //  var closestFilms = FindClosestFilms(filmName, fildict.Keys.ToList());

             Console.WriteLine("try this:");

               foreach (var film in closest)
              {
                  Console.WriteLine(film);
               }
        }
    }
}
List<string> LevinsteinList(string value, List<Film> checker)
    {
        var draft = new Dictionary<string, int>();
        var res = new List<string>();
        foreach (var VARIABLE in checker)
        {
            
        
        int[,] matrix = new int[value.Length + 1, VARIABLE.movie_title.Length + 1];
        for (int i = 0; i <= value.Length; i++)
        {
            matrix[i,0] = i;
        }
        for (int j = 0; j <= VARIABLE.movie_title.Length; j++)
        {
            matrix[0,j] = j;
        }
    
        for (int i = 1; i < value.Length+1; i++)
        {
            for (int j = 1; j <VARIABLE.movie_title.Length+1; j++)
            {
                int v1 = matrix[i - 1, j] + 1;
                int v2 = matrix[i, j - 1] + 1;
                int v3 = 0;
                int v4 = 0;
                int cost = 0;
                
                if (value[i-1]==VARIABLE.movie_title[j-1])
                {
                    v3 = matrix[i - 1, j - 1] ;
                    cost = 0;
                }
                else
                {
                    v3 = matrix[i - 1, j - 1]+1;
                    cost = 1;
                }
                if (i>1 && j>1)
                {
                    if (VARIABLE.movie_title[j-1] == value[i - 2] && VARIABLE.movie_title[j - 2] == value[i-1])
                    {
                        v4 = matrix[i - 2, j - 2]+cost; 
                    }
                    else
                    {
                        v4 = 999999999;
                    } 
                } else {
                    v4 = 999999999;
                }
                matrix[i,j] = Math.Min(Math.Min(v1, v2), Math.Min(v3,v4));
            }
        }
        var addd = matrix[value.Length, VARIABLE.movie_title.Length];
        if (!draft.ContainsKey(VARIABLE.movie_title))
        {
            draft.Add(VARIABLE.movie_title, addd); 
        }
         
        }
        var Sorted_Word_Points = draft.OrderBy(x => x.Value);
        var count = 0;
        foreach (var VARIABLE in Sorted_Word_Points)
        {
            count += 1;
            res.Add(VARIABLE.Key);
            if (count>2)
            {
                break;
            }
        }
        return res;
    }


public class Film
{
  
    [Name("genres")]
    public string? genres { get; set; }
    
    [Name("movie_id")]
    public string? movie_id { get; set; }
    
    [Name("movie_title")]
    public string? movie_title { get; set; }
}

public class Raiting
{
  
    [Name("rating_val")]
    public string? rating_val { get; set; }
    
    
    [Name("user_id")]
    public string? user_id { get; set; }
  
    [Name("movie_id")]
    public string? movie_id { get; set; }
    
    [Name("_id")]
    public string? _id { get; set; }
}

public class Users
{
    [Name("username")]
    public string? username { get; set; }
    
    
    [Name("_id")]
    public string? _id { get; set; }
}