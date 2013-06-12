open System
open System.Drawing
open System.Windows.Forms

let SKY_COLOUR = Color.Black
let INCREMENT_RA = 2. / 15. 
let INCREMENT_DEC = 2.
let DIMMEST_MAGNITUDE = 8.

// Global diagnostics stuff:
let stopWatch = new System.Diagnostics.Stopwatch()
let mutable diagnosticString = ""

type ViewPort =
    {
        mutable minRA : float
        mutable maxRA : float
        mutable minDec : float
        mutable maxDec : float
    }
    member p.WidthHrs = p.maxRA - p.minRA
    member p.HeightDegrees = p.maxDec - p.minDec
    member p.CentreRA = p.minRA + p.WidthHrs / 2.
    member p.CentreDec = p.minDec + p.HeightDegrees / 2.
    member p.Move incRA incDec = p.minRA <- p.minRA + incRA
                                 p.maxRA <- p.maxRA + incRA
                                 p.minDec <- p.minDec + incDec
                                 p.maxDec <- p.maxDec + incDec

type DoubleBufferedForm() as f = 
    inherit System.Windows.Forms.Form()

    do
        f.WindowState <- FormWindowState.Maximized
        f.FormBorderStyle <- FormBorderStyle.None

    let _graphics : Graphics = f.CreateGraphics(SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias)
    let _buffer = BufferedGraphicsManager.Current.Allocate(_graphics, f.DisplayRectangle)

    do 
        f.DoubleBuffered <- true

    member public f.Graphics = _graphics

let ColorForSpectrum (spectrum : string) =
    // http://www.vendian.org/mncharity/dir3/starcolor/
    if String.IsNullOrEmpty(spectrum) then
        Color.FromArgb(255, 255, 255, 255)
    else
        let spectrumClass = spectrum.[0]
        match spectrumClass with
        | 'O' -> Color.FromArgb(255, 155, 176, 255)  
        | 'B' -> Color.FromArgb(255, 170, 191, 255)  
        | 'A' -> Color.FromArgb(255, 202, 215, 255)  
        | 'F' -> Color.FromArgb(255, 248, 247, 255)  
        | 'G' -> Color.FromArgb(255, 255, 244, 234)  
        | 'K' -> Color.FromArgb(255, 255, 210, 161)  
        | 'M' -> Color.FromArgb(255, 255, 204, 111) 
        | _ ->   Color.FromArgb(255, 255, 255, 255)

let SizeForMagnitude (magnitude : Nullable<float>) =
    let SIRIUS_MAGNITUDE = -1.44
    20. - (magnitude.GetValueOrDefault(100.) - SIRIUS_MAGNITUDE) * 2.5 |> int

let DrawStar (form : DoubleBufferedForm) (viewPort : ViewPort) (star : StarsDatabase.dbSchema.ServiceTypes.StarLocations) =
    let x = (form.Width |> float) - (star.RA.Value - viewPort.minRA) / viewPort.WidthHrs * (form.Width |> float) |> int
    let y = form.Height - ((star.Dec.Value - viewPort.minDec) / viewPort.HeightDegrees * (form.Height |> float) |> int)
    let size = SizeForMagnitude star.Mag
    let color = ColorForSpectrum star.Spectrum
    use brush = new SolidBrush(color)
    form.Graphics.FillEllipse(brush, x, y, size, size) 

let DrawStars (form : DoubleBufferedForm) (viewPort : ViewPort) =
    stopWatch.Restart() 
    let stars = StarsDatabase.FindStarsTP viewPort.minRA viewPort.maxRA viewPort.minDec viewPort.maxDec DIMMEST_MAGNITUDE
    stopWatch.Stop()
    diagnosticString <- sprintf "(%i stars)" (stars |> Seq.length)

    stars
    |> Array.iter (fun star -> DrawStar form viewPort star)

let DrawLegend (form : DoubleBufferedForm) (viewPort : ViewPort) =
    use brush = new SolidBrush(Color.Green)
    use pen = new Pen(brush)
    use font = new Font(System.Drawing.FontFamily.GenericSansSerif, 25.f)
    form.Graphics.DrawString((sprintf "RA: %3.2f Dec: %3.2f (%i ms) %s" viewPort.CentreRA viewPort.CentreDec stopWatch.ElapsedMilliseconds diagnosticString), font, brush, 10.f, 10.f) 

let Refresh (form : DoubleBufferedForm) viewPort = 
    form.Graphics.Clear(SKY_COLOUR)
    DrawLegend form viewPort
    DrawStars form viewPort

let MoveForKey keyData (viewPort : ViewPort) form =
     match keyData with
    | Keys.Left ->  viewPort.Move INCREMENT_RA 0.;   Refresh form viewPort
    | Keys.Right -> viewPort.Move -INCREMENT_RA 0.;  Refresh form viewPort
    | Keys.Up ->    viewPort.Move 0. INCREMENT_DEC;  Refresh form viewPort
    | Keys.Down ->  viewPort.Move 0. -INCREMENT_DEC; Refresh form viewPort
    | Keys.Escape -> form.Close()
    | _ -> ()

let CreateForm() = 
    let form = new DoubleBufferedForm(BackColor=SKY_COLOUR)
    form.Show()
    form

[<EntryPoint>]
let main argv = 

    let form = CreateForm()

    let viewPort = {minRA = 0.0
                    maxRA = 2.0 * (form.Width |> float) / (form.Height |> float)
                    minDec = 0.0
                    maxDec = 30.}

    form.KeyDown |> Event.add (fun e -> MoveForKey e.KeyData viewPort form)

    Application.Run(form)

    0









/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Stored-procedure versions - needed because of different service type:
//
//let DrawStarSP (form : DoubleBufferedForm) (viewPort : ViewPort) (star : StarsDatabaseDemo.dbSchema.ServiceTypes.FindStarsResult) =
//    let x = (form.Width |> float) - (star.RA.Value - viewPort.minRA) / viewPort.WidthHrs * (form.Width |> float) |> int
//    let y = form.Height - ((star.Dec.Value - viewPort.minDec) / viewPort.HeightDegrees * (form.Height |> float) |> int)
//    let size = SizeForMagnitude star.Mag
//    let color = ColorForSpectrum star.Spectrum
//    use brush = new SolidBrush(color)
//    form.Graphics.FillEllipse(brush, x, y, size, size) 
//
//let DrawStarsSP (form : DoubleBufferedForm) (viewPort : ViewPort) =
//    stopWatch.Restart() 
//    let stars = StarsDatabaseDemo.FindStarsTPSP viewPort.minRA viewPort.maxRA viewPort.minDec viewPort.maxDec DIMMEST_MAGNITUDE
//    stopWatch.Stop()
//    diagnosticString <- sprintf "(%i stars)" stars.Length
//
//    stars
//    |> Array.iter (fun star -> DrawStarSP form viewPort star)
//
// End of stored procedure versions.

