pipeline {
    agent any
    
    environment {
        LOG_FILE = ""
        GIT_REPO = 'github.com/lauristi/ServerClipboard_API_Solution.git'
        BRANCH = 'master'
        PROJECT_NAME = 'ServerClipboard_API'
        PROJECT_PATH_ARCHIVE = 'ServerClipboard_API/ServerClipboard_API.csproj'
        PUBLISH_PATH = 'ServerClipboard_API/bin/Release/net8.0/publish'
        ARTIFACT_PATH = 'ServerClipboard_API/Artifact'
        DEPLOY_PATH = '/var/www/app/ServerClipboardProjects/ServerClipboard_API'
        DOTNET_ROOT = '/opt/dotnet'
    }

    stages {
        stage('Initialize log') {
            steps {
                script {
                    // Define o nome do arquivo de log com data e hora
                    env.LOG_FILE = "pipeline_${new Date().format('yyyyMMdd_HHmmss')}.log"
                    echo "Log file: ${env.LOG_FILE}"
                }
            }
        }

        stage('01- Checkout') {
            steps {
                script {
                    withCredentials([string(credentialsId: 'JENKINS_TOKEN', variable: 'GITHUB_TOKEN')]) {
                        try {
                            echo "01- Checkout" | tee -a $LOG_FILE
                            sh '''
                                git clone https://${GITHUB_TOKEN}@${GIT_REPO}
                                cd ServerClipboard_API_Solution
                                git checkout ${BRANCH}
                            '''
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
                        echo "02- Restore Dependencies" | tee -a $LOG_FILE
                        sh "${env.DOTNET_ROOT}/dotnet --version"
                        sh "${env.DOTNET_ROOT}/dotnet restore ${env.PROJECT_NAME}"
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
                        echo "03- Build Project" | tee -a $LOG_FILE
                        sh "${env.DOTNET_ROOT}/dotnet build ${env.PROJECT_NAME} --no-restore --configuration Debug"
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
                        echo "04- Test" | tee -a $LOG_FILE
                        sh "${env.DOTNET_ROOT}/dotnet test ${env.PROJECT_NAME} --no-build --verbosity normal"
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
                        echo "05- Publish" | tee -a $LOG_FILE
                        sh "${env.DOTNET_ROOT}/dotnet publish ${env.PROJECT_PATH_ARCHIVE} -c Release -o ${env.PUBLISH_PATH}"
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
                        echo "06- Package Artifacts" | tee -a $LOG_FILE
                        sh """
                            mkdir -p ${env.ARTIFACT_PATH}
                            cp -r ${env.PUBLISH_PATH}/* ${env.ARTIFACT_PATH}/
                        """
                        archiveArtifacts artifacts: "${env.ARTIFACT_PATH}/**", allowEmptyArchive: true
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
                        echo "07- Deploy on server" | tee -a $LOG_FILE
                        sh """
                            sudo -S cp -r "${env.ARTIFACT_PATH}"/* "${env.DEPLOY_PATH}/" && echo "Copy succeeded" || echo "Copy failed"
                            sudo chown -R www-data:www-data "${env.DEPLOY_PATH}/" && echo "Chown succeeded" || echo "Chown failed"
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
            script {
                archiveArtifacts artifacts: "${env.LOG_FILE}", allowEmptyArchive: true
                cleanWs()
            }
        }
    }
}

def TratarErro(Exception e) {
    currentBuild.result = 'FAILURE'
    echo "--------------------------------------------------------------"
    echo "Deploy failed: ${e.message}"
    echo "--------------------------------------------------------------"
    error('Deploy failed')
    echo "--------------------------------------------------------------"
}
