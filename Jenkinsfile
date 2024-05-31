pipeline {
    agent any

    environment {
        GIT_REPO = 'https://github.com/lauristi/ServerClipboard_API_Solution.git'  // URL do reposit�rio Git
        BRANCH = 'master'  // Branch do reposit�rio que ser� usada
        SOLUTION_PATH = 'ServerClipboard_API'  // Caminho para a solu��o no reposit�rio
        PROJECT_PATH = 'ServerClipboard_API/ServerClipboard_API.csproj'  // Caminho para o arquivo de projeto espec�fico
        BUILD_PATH = 'ServerClipboard_API/bin/Debug/net8.0'  // Caminho para os arquivos de build
        PUBLISH_PATH = 'ServerClipboard_API/bin/Release/net8.0/publish'  // Caminho para os arquivos publicados
        ARTIFACT_PATH = 'ServerClipboard_API/Artifact'  // Caminho onde os artefatos ser�o armazenados
        DEPLOY_DIR = '/var/www/app/ServerClipboard_API'  // Diret�rio de implanta��o no servidor
    }
        
    stages {
        stage('Checkout') {
            steps {
                // Clona o reposit�rio Git especificado na branch definida
                git branch: "${env.BRANCH}", url: "${env.GIT_REPO}"
            }
        }

        stage('Setup .NET') {
            steps {
                // Baixa e instala o .NET SDK necess�rio
                sh 'wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh'
                sh 'chmod +x dotnet-install.sh'
                sh './dotnet-install.sh -c 8.0'
                sh 'export PATH=$PATH:$HOME/.dotnet'  // Adiciona o .NET ao PATH do sistema
            }
        }

        stage('Restore Dependencies') {
            steps {
                // Restaura as depend�ncias do projeto
                sh "dotnet restore ${env.SOLUTION_PATH}"
            }
        }

        stage('Build') {
            steps {
                // Compila o projeto na configura��o Debug
                sh "dotnet build ${env.SOLUTION_PATH} --no-restore --configuration Debug"
            }
        }

        stage('Test') {
            steps {
                // Executa os testes do projeto
                sh "dotnet test ${env.SOLUTION_PATH} --no-build --verbosity normal"
            }
        }

        stage('Publish') {
            steps {
                // Publica o projeto, criando a vers�o de release
                sh "dotnet publish ${env.PROJECT_PATH} -c Release -o ${env.PUBLISH_PATH}"
            }
        }

        stage('Package Artifacts') {
            steps {
                script {
                    // Cria o diret�rio de artefatos e copia os arquivos publicados para l�
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
            steps {
                script {
                    // Garante que o diret�rio de implanta��o exista e copia os arquivos para l�
                    sh """
                        sudo mkdir -p ${env.DEPLOY_DIR}
                        sudo cp -r ${env.ARTIFACT_PATH}/* ${env.DEPLOY_DIR}/
                        sudo chown -R www-data:www-data ${env.DEPLOY_DIR}  // Define permiss�es apropriadas
                    """
                }
            }
        }
    }

    post {
        always {
            // Limpa o workspace do Jenkins ap�s a execu��o do pipeline
            cleanWs()
        }
    }
}
