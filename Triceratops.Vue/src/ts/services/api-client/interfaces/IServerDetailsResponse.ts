import IContainerResponse from './IContainerResponse';

export default interface IServerDetailsResponse {
    serverId: string;
    success: boolean;
    error: string;
    name: string;
    slug: string;
    serverType: number;
    containers: IContainerResponse[];
}