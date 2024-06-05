pipeline {
    agent any

    environment {
        LOG_FILE = "pipeline.log"
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

        stage('clean') {
        steps {
                script {
                    // Remove o diret�rio existente se ele j� existir
                    // Limpar diret�rio de builds antigos
                    // Limpar diret�rio de artefatos antigos
                    sh '''
                        sh rm -rf ServerClipboard_API_Solution
                        sh rm -rf build/*
                        sh rm -rf artifacts/*
                   ''' 

                }
            }
        }

        stage('01- Checkout') {
            steps {
                script {
                    withCredentials([string(credentialsId: 'JENKINS_TOKEN', variable: 'GITHUB_TOKEN')]) {
                        try {
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
                        sh '''
                            ${env.DOTNET_ROOT}/dotnet --version 
                            ${env.DOTNET_ROOT}/dotnet restore ${PROJECT_NAME} 
                        '''
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
                        sh '''
                            ${env.DOTNET_ROOT}/dotnet build ${PROJECT_NAME} --no-restore --configuration Debug 
                        '''
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
                        sh '''
                            ${env.DOTNET_ROOT}/dotnet test ${PROJECT_NAME} --no-build --verbosity normal 
                        '''
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
                        sh '''
                            ${env.DOTNET_ROOT}/dotnet publish ${PROJECT_PATH_ARCHIVE} -c Release -o ${PUBLISH_PATH} 
                        '''
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
                        sh '''
                            mkdir -p ${ARTIFACT_PATH}
                            cp -r ${PUBLISH_PATH}/* ${ARTIFACT_PATH}/ 
                        '''
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
                        sh '''
                            sudo -S cp -r "${ARTIFACT_PATH}"/* "${DEPLOY_PATH}/" && echo "Copy succeeded" || echo "Copy failed"
                            sudo chown -R www-data:www-data "${DEPLOY_PATH}/" && echo "Chown succeeded" || echo "Chown failed"
                        '''
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
                archiveArtifacts artifacts: "${LOG_FILE}", allowEmptyArchive: true
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
