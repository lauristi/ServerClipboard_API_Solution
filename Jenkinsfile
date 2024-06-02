pipeline {
    agent any

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

    stages {
        stage('01- Checkout') {
            steps {
                script {
                    withCredentials([string(credentialsId: 'JENKINS_TOKEN', variable: 'GITHUB_TOKEN')]) {
                        try {
                            sh 'git clone https://${GITHUB_TOKEN}@github.com/lauristi/ServerClipboard_API_Solution.git'
                            sh "cd ServerClipboard_API_Solution && git checkout ${env.BRANCH}"
                        } catch (Exception e) {
                            TratarErro(e)
                        }
                    }
                }
            }
        }

        stage('02- Restore Dependencies') {
            steps {
                script {
                    try {
                        sh "${DOTNET_ROOT}/dotnet --version"
                        sh "${DOTNET_ROOT}/dotnet restore ${SOLUTION_PATH}"
                    } catch (Exception e) {
                        TratarErro(e)
                    }
                }
            }
        }

        stage('03- Build') {
            steps {
                script {
                    try {
                        sh "${DOTNET_ROOT}/dotnet build ${SOLUTION_PATH} --no-restore --configuration Debug"
                    } catch (Exception e) {
                        TratarErro(e)
                    }
                }
            }
        }

        stage('04- Test') {
            steps {
                script {
                    try {
                        sh "${DOTNET_ROOT}/dotnet test ${SOLUTION_PATH} --no-build --verbosity normal"
                    } catch (Exception e) {
                        TratarErro(e)
                    }
                }
            }
        }

        stage('05- Publish') {
            steps {
                script {
                    try {
                        sh "${DOTNET_ROOT}/dotnet publish ${PROJECT_PATH} -c Release -o ${PUBLISH_PATH}"
                    } catch (Exception e) {
                        TratarErro(e)
                    }
                }
            }
        }

        stage('06- Package Artifacts') {
            steps {
                script {
                    try {
                        sh """
                          mkdir -p ${ARTIFACT_PATH}
                          cp -r ${PUBLISH_PATH}/* ${ARTIFACT_PATH}/
                      """
                        archiveArtifacts artifacts: "${ARTIFACT_PATH}/**", allowEmptyArchive: true
                    } catch (Exception e) {
                        TratarErro(e)
                    }
                }
            }
        }

        stage('07- Deploy on server') {
            steps {
                script {
                    try {
                        sh """
                         sudo -S cp -r "${ARTIFACT_PATH}"/* "${DEPLOY_DIR}/" && echo "Copy succeeded" || echo "Copy failed"
                         sudo chown -R www-data:www-data "${DEPLOY_DIR}/" && echo "Chown succeeded" || echo "Chown failed"
                      """
                    } catch (Exception e) {
                        TratarErro(e)
                    }
                }
            }
        }
    }

    post {
        always {
            cleanWs()
        }
    }
}

def TratarErro(Exception e) {
    currentBuild.result = 'FAILURE'
    echo "Deploy failed: ${e.message}"
    error('Deploy failed')
}
