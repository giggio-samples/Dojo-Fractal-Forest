open System
open System.Drawing
open System.Windows.Forms

let formWidth, formHeight = 1200, 800
let form = new Form(Width = formWidth, Height = formHeight)
let box = new PictureBox(BackColor = Color.White, Dock = DockStyle.Fill)
let image = new Bitmap(formWidth, formHeight)
let graphics = Graphics.FromImage(image)
graphics.SmoothingMode <- System.Drawing.Drawing2D.SmoothingMode.HighQuality
let redBrush = new SolidBrush(Color.FromArgb(255, 0, 0))
let blueBrush = new SolidBrush(Color.FromArgb(0, 0, 255))
let darkGreenBrush = new SolidBrush(Color.FromArgb(22, 91, 19))
let greenBrush = new SolidBrush(Color.FromArgb(18, 114, 15))
let brownBrush = new SolidBrush(Color.FromArgb(102, 51, 0))
let blackBrush = new SolidBrush(Color.FromArgb(0, 0, 0))

box.Image <- image
form.Controls.Add(box)

type Direction =
    | Left
    | Right
    | Up
// Compute the endpoint of a line
// starting at x, y, going at a certain angle
// for a certain length.
let endpoint x y angle length =
    x + length * cos angle,
    y + length * sin angle

let flip x = (float)formHeight - x

let drawLine (target : Graphics) (brush : Brush)
             (x : float) (y : float)
             (angle : float) (length : float) (width : float) =
    let x_end, y_end = endpoint x y angle length
    let origin = new PointF((single)x, (single)(y |> flip))
    let destination = new PointF((single)x_end, (single)(y_end |> flip))
    let pen = new Pen(brush, (single)width)
    target.DrawLine(pen, origin, destination)
    x_end, y_end

let draw x y angle length width direction =
    match direction with
        | Left -> drawLine graphics greenBrush x y angle length width
        | Right -> drawLine graphics darkGreenBrush x y angle length width
        | _ -> drawLine graphics brownBrush x y angle length width

let pi = Math.PI
let finalDepth = 15
let width = 10.
let baseLength = 65.
let variation = 0.11
let rec branch depth x y direction baseAngle =
    let depthRatio = ((float)finalDepth - (float)depth + 1.) / (float)finalDepth
    let newVariation = variation * depthRatio
    let opennessAngleMultiplier = if depth % 2 = 0 then 1.5 else 1.
    let leftAngleModifier, rightAngleModifier = baseAngle + (newVariation * opennessAngleMultiplier), baseAngle - (newVariation * opennessAngleMultiplier)
    let leftAngle, rightAngle =  pi*leftAngleModifier, pi*rightAngleModifier
    let newWidth = width * ( 1. / ( (float)depth + 1. ) )
    let rightLength = baseLength * 1. * depthRatio
    let leftLength = if depth % 2 = 1 then rightLength else rightLength * 0.7
    let leftx, lefty = draw x y leftAngle leftLength newWidth Left
    let rightx, righty = draw x y rightAngle rightLength newWidth Right
    let newDepth = depth + 1
    if newDepth <= finalDepth then
        branch newDepth leftx lefty Left leftAngleModifier
        branch newDepth rightx righty Right rightAngleModifier

let trunkEndX, trunkEndY = draw ((float)formWidth/2.) 50. (pi*(0.5)) 100. width Up
branch 1 trunkEndX trunkEndY Up 0.5
form.ShowDialog()