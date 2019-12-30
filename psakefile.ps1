properties {
    $base_dir = resolve-path .
    $db_dir = "$base_dir\db"
    $solution_name = "MarkdownWebsite"
    $solution_dir = "$base_dir"
	$solution_path = "$solution_dir\$solution_name.sln"
	$runtime_id = "win10-x64"
	$project_dir = "$solution_dir\$solution_name"
}

#these tasks are for developers to run
task default -depends Clean, Compile

task Clean {
    exec { & dotnet clean $solution_path }
}

task Compile {
    exec { & dotnet build $solution_path }
}

task Publish {
	exec { & dotnet clean -c Release $project_dir }
	exec { & dotnet publish -c Release -r $runtime_id $main_project_dir /p:PublishSingleFile=true /p:SelfContained=false }
}
