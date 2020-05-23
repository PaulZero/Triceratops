import IServerListResponse from './interfaces/IServerListResponse';

export default class ApiClient {
    public getServerList(): IServerListResponse {
        return {
            success: true,
            error: null,
            servers: [
                {
                    success: true,
                    error: 'no such thing',
                    serverId: 'blah-blah-blah',
                    name: 'Some Server',
                    serverType: 0,
                    slug: 'some-server',
                    containers: [
                        {
                            id: 'blaaaaaaaaaah',
                            name: 'some-container',
                            dockerId: 'gdfgdfgdfgdf',
                            serverContainerState: 1,
                            creationDate: new Date(2020, 11, 12, 12, 0, 0)
                        }
                    ]
                }
            ]
        };
    }
}