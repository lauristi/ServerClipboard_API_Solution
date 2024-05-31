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
    }

    stages {
        stage('Checkout') {
            steps {
                // Clona o repositório público
                git branch: "${env.BRANCH}", url: "${env.GIT_REPO}"
            }
        }

        stage('Setup .NET') {
            steps {
                // Baixa e instala o .NET SDK necessário
                sh 'wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh'
                sh 'chmod +x dotnet-install.sh'
                sh './dotnet-install.sh -c 8.0'
                sh 'export PATH=$PATH:$HOME/.dotnet'  // Adiciona o .NET ao PATH do sistema
            }
        }

        stage('Restore Dependencies') {
            steps {
                // Restaura as dependências do projeto
                sh "dotnet restore ${env.SOLUTION_PATH}"
            }
        }

        stage('Build') {
            steps {
                // Compila o projeto na configuração Debug
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
                // Publica o projeto, criando a versão de release
                sh "dotnet publish ${env.PROJECT_PATH} -c Release -o ${env.PUBLISH_PATH}"
            }
        }

        stage('Package Artifacts') {
            steps {
                script {
                    // Cria o diretório de artefatos e copia os arquivos publicados para lá
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
                    // Garante que o diretório de implantação exista e copia os arquivos para lá
                    sh """
                        sudo mkdir -p ${env.DEPLOY_DIR}
                        sudo cp -r ${env.ARTIFACT_PATH}/* ${env.DEPLOY_DIR}/
                        sudo chown -R www-data:www-data ${env.DEPLOY_DIR}  // Define permissões apropriadas
                    """
                }
            }
        }
    }

    post {
        always {
            // Limpa o workspace do Jenkins após a execução do pipeline
            cleanWs()
        }
    }
}
