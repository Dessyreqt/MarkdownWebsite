properties {
    $base_dir = resolve-path .
    $db_dir = "$base_dir\db"
    $solution_name = "MarkdownWebsite"
    $solution_dir = "$base_dir"
	$solution_path = "$solution_dir\$solution_name.sln"
	$runtime_id = "win-x64"
    $project_dir = "$solution_dir\$solution_name"
    
    $output_dir = "$project_dir\bin\Debug\netcoreapp3.1\win-x64"
    $output_exe = "$output_dir\MarkdownWebsite.exe"

    $example_src = "$base_dir\example\src"
    $example_static = "$base_dir\example\static"
    $example_layout = "$base_dir\example\layout"
}

#these tasks are for developers to run
task default -depends Clean, Compile, BuildExampleWebsite

task Clean {
    exec { & dotnet clean $solution_path }
}

task Compile {
    exec { & dotnet build $solution_path }
}

task BuildExampleWebsite {
    exec { & $output_exe website -i $example_src -o $example_static -l $example_layout }
}

task Publish {
	exec { & dotnet clean -c Release $project_dir }
	exec { & dotnet publish -c Release -r $runtime_id $project_dir /p:SelfContained=false }
}
