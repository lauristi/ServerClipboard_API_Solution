pipeline {
  agent any
  stages {
    stage('Checkout') {
      steps {
        git(branch: "${env.BRANCH}", url: "${env.GIT_REPO}")
      }
    }

    stage('Setup .NET') {
      steps {
        sh 'wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh'
        sh 'chmod +x dotnet-install.sh'
        sh './dotnet-install.sh -c 8.0'
        sh 'export PATH=$PATH:$HOME/.dotnet'
      }
    }

    stage('Restore Dependencies') {
      steps {
        sh "dotnet restore ${env.SOLUTION_PATH}"
      }
    }

    stage('Build') {
      steps {
        sh "dotnet build ${env.SOLUTION_PATH} --no-restore --configuration Debug"
      }
    }

    stage('Test') {
      steps {
        sh "dotnet test ${env.SOLUTION_PATH} --no-build --verbosity normal"
      }
    }

    stage('Publish') {
      steps {
        sh "dotnet publish ${env.PROJECT_PATH} -c Release -o ${env.PUBLISH_PATH}"
      }
    }

    stage('Package Artifacts') {
      steps {
        script {
          sh """
          mkdir -p ${env.ARTIFACT_PATH}
          cp -r ${env.PUBLISH_PATH}/* ${env.ARTIFACT_PATH}/
          """
          // Arquiva os artefatos no Jenkins
          archiveArtifacts artifacts: "${env.ARTIFACT_PATH}/**", allowEmptyArchive: true
        }

      }
    }

    stage('Deploy') {
      agent any
      steps {
        script {
          sh """
          sudo mkdir -p ${env.DEPLOY_DIR}
          sudo cp -r ${env.ARTIFACT_PATH}/* ${env.DEPLOY_DIR}/
          sudo chown -R www-data:www-data ${env.DEPLOY_DIR}  // Define permissões apropriadas
          """
        }

      }
    }

  }
  environment {
    REPO_OWNER = 'lauristi'
    REPO_NAME = 'ServerClipboard_API_Solution'
    GIT_REPO = 'https://github.com/lauristi/ServerClipboard_API_Solution.git'
    BRANCH = 'master'
    SOLUTION_PATH = 'ServerClipboard_API'
    PROJECT_PATH = 'ServerClipboard_API/ServerClipboard_API.csproj'
    BUILD_PATH = 'ServerClipboard_API/bin/Debug/net8.0'
    PUBLISH_PATH = 'ServerClipboard_API/bin/Release/net8.0/publish'
    ARTIFACT_PATH = 'ServerClipboard_API/Artifact'
    DEPLOY_DIR = '/var/www/app/ServerClipboard_API'
  }
  post {
    always {
      cleanWs()
    }

  }
}