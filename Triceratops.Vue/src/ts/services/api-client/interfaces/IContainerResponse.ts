export default interface IContainerResponse {
    id: string;
    dockerId: string;
    name: string;
    serverContainerState: number;
    creationDate: Date;
}