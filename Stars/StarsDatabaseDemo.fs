module StarsDatabaseDemo

open System
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open System.Data.SqlClient

type dbSchema = SqlDataConnection<"Data Source=KITSBLACKLAPTOP;Initial Catalog=Stars;Integrated Security=SSPI;">

let FindStars' minRA maxRA minDec maxDec brighterThan =
    let db = dbSchema.GetDataContext()

    db.StarLocations
    |> Seq.filter (fun star -> star.RA ?> minRA
                               && star.RA ?< maxRA
                               && star.Dec ?> minDec
                               && star.Dec ?< maxDec
                               && star.Mag ?< brighterThan
                   )
    |> Array.ofSeq

let FindStars'' minRA maxRA minDec maxDec brighterThan =
    let db = dbSchema.GetDataContext()

    query {
            for star in db.StarLocations do
            where ( star.RA ?> minRA
                    && star.RA ?< maxRA
                    && star.Dec ?> minDec
                    && star.Dec ?< maxDec
                    && star.Mag ?< brighterThan )
            sortByNullable star.Mag
            select star
          }
    |> Array.ofSeq

let FindStars minRA maxRA minDec maxDec brighterThan =
    let db = dbSchema.GetDataContext()
    let nullable value = new Nullable<_>(value)
    db.FindStars((nullable minRA), (nullable maxRA),
                 (nullable minDec), (nullable maxDec), 
                 (nullable brighterThan))
    |> Array.ofSeq




