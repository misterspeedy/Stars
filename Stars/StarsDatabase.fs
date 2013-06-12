module StarsDatabase

#if INTERACTIVE
#r "System.Data.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"
#endif

open System
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq
open System.Data.SqlClient

type dbSchema = SqlDataConnection<"Data Source=.;Initial Catalog=Stars;Integrated Security=SSPI;">

/// Inclusive 'between' operator with nullable value argument.
let (?>=<) (x : Nullable<_>) (a, b) =
    x.HasValue &&
    x.Value >= a && x.Value <= b

/// Find stars in a given view port (type provider version).
let FindStarsTP minRA maxRA minDec maxDec brighterThan =
    let db = dbSchema.GetDataContext()

    db.StarLocations
    |> Seq.filter (fun star -> star.RA ?>=< (minRA, maxRA) 
                               && star.Dec ?>=< (minDec, maxDec)
                               && star.Mag ?< brighterThan)
    |> Array.ofSeq
    |> Array.sortBy (fun star -> star.Mag.GetValueOrDefault(0.0))

/// Find stars in a given view port (type provider and query expression version).
let FindStarsQueryExpr minRA maxRA minDec maxDec brighterThan =
    let db = dbSchema.GetDataContext()

    query { for star in db.StarLocations do
            where (
                   star.RA ?>= minRA && star.RA ?<= maxRA
                   && star.Dec ?>= minDec && star.Dec ?<= maxDec 
                   && star.Mag ?< brighterThan
                  )
            sortByNullable star.Mag
            select star
    }
    |> Array.ofSeq

/// Find stars in a given view port (type provider and stored procedure version).
let FindStarsTPSP minRA maxRA minDec maxDec brighterThan =
    let db = dbSchema.GetDataContext()
    let nullable value = new Nullable<_>(value)

    db.FindStars( (nullable minRA), (nullable maxRA),
                    (nullable minDec), (nullable maxDec),
                    (nullable brighterThan) )
    |> Array.ofSeq
    |> Array.sortBy (fun star -> star.Mag.GetValueOrDefault(0.0))