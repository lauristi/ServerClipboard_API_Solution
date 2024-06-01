pipeline {
  agent any
  stages {
    stage('Checkout') {
      steps {
        script {
          withCredentials([string(credentialsId: 'JENKINS_TOKEN', variable: 'GITHUB_TOKEN')]) {
            sh 'git clone https://${GITHUB_TOKEN}@github.com/lauristi/ServerClipboard_API_Solution.git'
            sh "cd ServerClipboard_API_Solution && git checkout ${env.BRANCH}"
          }
        }
      }
    }
    stage('Restore Dependencies') {
      steps {
        sh "${env.DOTNET_ROOT}/dotnet --version"
        sh "${env.DOTNET_ROOT}/dotnet restore ${env.SOLUTION_PATH}"
      }
    }
    stage('Build') {
      steps {
        sh "${env.DOTNET_ROOT}/dotnet build ${env.SOLUTION_PATH} --no-restore --configuration Debug"
      }
    }
    stage('Test') {
      steps {
        sh "${env.DOTNET_ROOT}/dotnet test ${env.SOLUTION_PATH} --no-build --verbosity normal"
      }
    }
    stage('Publish') {
      steps {
        sh "${env.DOTNET_ROOT}/dotnet publish ${env.PROJECT_PATH} -c Release -o ${env.PUBLISH_PATH}"
      }
    }
    stage('Package Artifacts') {
      steps {
        script {
          sh """
          mkdir -p ${env.ARTIFACT_PATH}
          cp -r ${env.PUBLISH_PATH}/* ${env.ARTIFACT_PATH}/
          """
          archiveArtifacts artifacts: "${env.ARTIFACT_PATH}/**", allowEmptyArchive: true
        }
      }
    }
    stage('Deploy') {
      steps {
        script {
          withCredentials([string(credentialsId: 'SUDO_PASSWORD', variable: 'SUDO_PASSWORD')]) {
            sh '''
              echo "${SUDO_PASSWORD}" | sudo -S mkdir -p "${DEPLOY_DIR}"
              sudo cp -r "${ARTIFACT_PATH}"/* "${DEPLOY_DIR}/"
              sudo chown -R www-data:www-data "${DEPLOY_DIR}/"
            '''
          }
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
    DOTNET_ROOT = '/opt/dotnet'
    PATH = "${DOTNET_ROOT}:${env.PATH}"
  }
  post {
    always {
      cleanWs()
    }
  }
}
