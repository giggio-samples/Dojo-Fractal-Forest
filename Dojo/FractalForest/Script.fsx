open System
open System.Drawing
open System.Windows.Forms

let finalDepth = 15
let baseWidth = 10.
let baseLength = 65.
let baseAngleVariation = 0.11
let formWidth, formHeight = 1200, 800

let form = new Form(Width = formWidth, Height = formHeight)
form.WindowState <- FormWindowState.Maximized
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

type Direction =
    | Left
    | Right
    | Up

let draw x y angle length width direction =
    match direction with
        | Left -> drawLine graphics greenBrush x y angle length width
        | Right -> drawLine graphics darkGreenBrush x y angle length width
        | _ -> drawLine graphics brownBrush x y angle length width

let pi = Math.PI
let rec branch depth x y direction baseAngle =
    let depthRatio = ((float)finalDepth - (float)depth + 1.) / (float)finalDepth
    let angleVariation = baseAngleVariation * depthRatio
    let opennessAngleMultiplier = if depth % 2 = 0 then 1.5 else 1.
    let leftAngleModifier = baseAngle + (angleVariation * opennessAngleMultiplier)
    let rightAngleModifier =  baseAngle - (angleVariation * opennessAngleMultiplier)
    let leftAngle, rightAngle =  pi*leftAngleModifier, pi*rightAngleModifier
    let width = baseWidth * ( 1. / ( (float)depth + 1. ) )
    let rightLength = baseLength * 0.9 * depthRatio
    let leftLength = if depth % 2 = 1 then rightLength else rightLength * 0.7
    let leftx, lefty = draw x y leftAngle leftLength width Left
    let rightx, righty = draw x y rightAngle rightLength width Right
    let newDepth = depth + 1
    if newDepth <= finalDepth then
        branch newDepth leftx lefty Left leftAngleModifier
        branch newDepth rightx righty Right rightAngleModifier

let trunkEndX, trunkEndY = draw ((float)formWidth/2.) 50. (pi*(0.5)) 100. baseWidth Up
branch 1 trunkEndX trunkEndY Up 0.5
form.Show()