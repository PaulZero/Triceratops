import { Action, Module, VuexModule, Mutation } from 'vuex-module-decorators';

@Module
export default class ApiStore extends VuexModule  {
    private _name: string = 'steve';

    public get name() {
        return this._name;
    }

    @Mutation
    public changeName(name: string): void {
        this._name = name;
    }

    @Action({ })
    activateBarry(): void {
        this.context.commit('changeName', 'Barry');
    }
}