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

var readerRaiting = new StreamReader(csvFilePathRatings);
var csvRaiting = new CsvReader(readerRaiting, config);
//var RaitingList = new List<Raiting>();
var recordsRaitings = csvRaiting.GetRecords<Raiting>().ToList();



var readerUser = new StreamReader(csvFilePathRatings);
var csvUSer = new CsvReader(readerRaiting, config);
var recordsUsers = csvRaiting.GetRecords<Users>().ToList();
//RaitingList = recordsRaitings.Where(x => dict.ContainsKey(x.movie_id)).ToList();

//.Replace("[", "").Replace("]", "").Replace(""", "").Split(",");

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



double[,] UserMatrix = new Double[recordsUsers.Count, 8];

//var dictUser=RaitingList.ToDictionary(x => x.user_id, x => x);

/*
var readerUser = new StreamReader(csvFilePathUser);
var csvUsers = new CsvReader(readerUser, config);
var UserList = new List<Users>();
var recordsUser = csvUsers.GetRecords<Users>();*/


/*
public class Point
{
    public string name{ get; set; }
    public int? Drama { get; set; }
    public int? Comedy { get; set; }
    public int? Documentary { get; set; }
    public int? Thriller { get; set; }
    public int? Horror{ get; set; }
    public int? Romance{ get; set; }
    public int? Action{ get; set; }
    public int? Animation{ get; set; }
    public int? Crime{ get; set; }
    public int? Music{ get; set; }
    public int? Adventure{ get; set; }
    public int? Family{ get; set; }
    public int? Science_Fiction{ get; set; }
    public int? Fantasy{ get; set; }
    public int? Mystery{ get; set; }
    public int? TV_Movie{ get; set; }
    public int? History{ get; set; }
    public int? War{ get; set; }
    public int? Western{ get; set; }
}*/

public class Film
{
  
    [Name("genres")]
    public string? genres { get; set; }
    
    [Name("movie_id")]
    public string? movie_id { get; set; }
    
    [Name("movie_title")]
    public string? movie_title { get; set; }

    public string ToString()
    {
        if (genres!=null && movie_title!=null)
        {
            return genres + " " + movie_title+ " "+movie_id;   
        }
        else
        {
            return "nothing";
        }
        
    }
    
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